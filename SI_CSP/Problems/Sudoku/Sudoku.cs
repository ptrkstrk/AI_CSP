using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SI_CSP.Problems.Sudoku
{
    public class Sudoku : ProblemDiagnostics, IProblem
    {
        //Algorithm
        public SudokuVariable[][] Board { get; set; } = new SudokuVariable[9][];
        public List<int[][]> Solutions { get; set; } = new List<int[][]>();
        public List<SudokuVariable> Variables { get; set; }
        public Dictionary<SudokuVariable, List<SudokuVariable>> relatedFields{get;set;} = new Dictionary<SudokuVariable, List<SudokuVariable>>();
        bool BackTrack = false;
        //public List<SudokuVariable> CurrBranch { get; set; } = new List<SudokuVariable>();
        private Func<SudokuVariable> SelectNextVariable;
        VariableHeuristics varHeur;
        ValueHeuristics valHeur;
        public int InstanceID;
        public float Difficulty;


        public Sudoku(SudokuVariable[][] board, int instanceID, float difficulty)
        {
            InstanceID = instanceID;
            Difficulty = difficulty;
            Board = board;
            SelectNextVariable = SelectNextFreeVariable;
            InitVariables();
            InitConstraints();
            ApplyVariableHeuristics(VariableHeuristics.MIN_REMAINING_VALUES_DYN);
            ApplyValueHeuristics(ValueHeuristics.LEAST_CONSTRAINING_VALUE);
        }

        public void InitVariables()
        {
            Variables = new List<SudokuVariable>();
            for (int i = 0; i < 9; i++)
                Variables.AddRange(Board[i]);
        }

        public void RunBacktracking()
        {
            if (firstRun)
            {
                BackTrack = true;
                StartTiming();
            }
                
            SudokuVariable currElem = SelectNextVariable();
            if (currElem == null)
            {
                if (Solutions.Count == 0)
                    SaveFirstSolutionData();

                Solutions.Add(
                    Board.Select(
                        s => s.Select(v => v.Value).ToArray())
                    .ToArray());
            }
            else
            {
                //OrderDomain(currElem);
                foreach (int value in currElem.Domain)
                {
                    currElem.Value = value;
                    TotalNodesVisited++;
                    bool correct = CheckConstraints(currElem);
                    if (correct)
                        RunBacktracking();
                }
                currElem.Value = 0;
            }
            TotalReturns++;// nawrot
            TotalTime = stopwatch.Elapsed.TotalSeconds;
        }

        public void RunForwardChecking()
        {
            if (firstRun)
            {
                RemoveInitiallyBlockedValuesFromDomains();
                StartTiming();
            }

            SudokuVariable currElem = SelectNextVariable();
            if (currElem == null)
            {
                if (Solutions.Count == 0)
                    SaveFirstSolutionData();

                Solutions.Add(
                    Board.Select(
                        s => s.Select(v => v.Value).ToArray())
                    .ToArray());
                return;
            }

            foreach (int value in currElem.Domain)
            {
                currElem.Value = value;
                TotalNodesVisited++;
                
                FilterDomainsOfAllRelatedFields(currElem);
                if (!IsAnyDomainEmpty())
                    RunForwardChecking();
            }
            currElem.Value = 0; // nawrot - brak kolejnej wartosci
            FilterDomainsOfAllRelatedFields(currElem);
            TotalReturns++;
            TotalTime = stopwatch.Elapsed.TotalSeconds;
        }

        private void InitConstraints()
        {
            foreach (SudokuVariable variable in Variables) {
                relatedFields.Add(variable, new List<SudokuVariable>());
                AddFromRow(variable);
                AddFromCol(variable);
                AddFromSquare(variable);
                relatedFields[variable] = relatedFields[variable].Distinct().ToList();
            }
        }

        private void AddFromRow(SudokuVariable variable)
        {
            int row = variable.Row;
            for (int i = 0; i < 9; i++)
                if (i != variable.Col)
                    relatedFields[variable].Add(Board[row][i]);
        }

        private void AddFromCol(SudokuVariable variable)
        {
            int col = variable.Col;
            for (int i = 0; i < 9; i++)
                if (i != variable.Row)
                    relatedFields[variable].Add(Board[i][col]);
        }

        private void AddFromSquare(SudokuVariable variable)
        {
            int row = variable.Row;
            int col = variable.Col;

            int box_start_row = (row / 3) * 3;
            int box_start_col = (col / 3) * 3;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (box_start_row + i != row || box_start_col + j != col)
                        relatedFields[variable].Add(Board[box_start_row + i][box_start_col + j]);
        }

        private bool IsAnyDomainEmpty()
        {
            return Variables.Any(v => v.Domain.Count == 0);
        }

        //dynamiczna heurystyka - wybierz zmienna o aktualnie najmniejszej liczbie dozwolonych wartości
        private SudokuVariable SelectNextVariableMRV()
        {
            if (BackTrack)
            {
                SudokuVariable nextVar = null;
                int maxExcludedValues = 0;
                foreach (SudokuVariable variable in Variables)
                {
                    if (variable.IsEmpty())
                    {

                        List<int> excludedValues = new List<int>();
                        foreach (SudokuVariable related in relatedFields[variable])
                            if (!related.IsEmpty() && !excludedValues.Contains(related.Value))
                                excludedValues.Add(related.Value);
                        if (excludedValues.Count > maxExcludedValues)
                        {
                            maxExcludedValues = excludedValues.Count;
                            nextVar = variable;
                        }
                    }
                }
                return nextVar;
            }
            else
                return Variables.OrderBy(v => v.Domain.Count).FirstOrDefault(v => v.IsEmpty());
        }

        //statyczna heurystyka - wybierz pierwszą wolna zmienną z listy posortowanej na poczatku działania metody
        private SudokuVariable SelectNextFreeVariable()
        {
            return Variables.FirstOrDefault(v => v.IsEmpty());
        }

        public bool CheckConstraints(SudokuVariable variable)
        {
            foreach (SudokuVariable related in relatedFields[variable])
                if (related.Value == variable.Value)
                    return false;
            
            return true;
        }

        public void PresentSolutions()
        {
            foreach (int[][] board in Solutions)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                        sb.Append($"{board[i][j]} ");
                    sb.Append("\n");
                }
                string presentation = sb.ToString();
                Console.WriteLine(presentation);
            }
        }

        public void PresentCurrBoard()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                    sb.Append($"{Board[i][j].Value} ");
                sb.Append("\n");
            }
            string presentation = sb.ToString();
            Console.WriteLine(presentation);
        }

        public void PresentPerformance()
        {
            base.PresentPerformance(Solutions.Count);
        }

        public void ApplyVariableHeuristics(VariableHeuristics heur)
        {
            varHeur = heur;
            if (heur == VariableHeuristics.DEFINITION_ORDER)
                return;
            if (heur == VariableHeuristics.RANDOM)
            {
                Random rand = new Random(10);
                Variables = Variables.OrderBy(v => rand.Next()).ToList();
            }

            if (heur == VariableHeuristics.MIN_REMAINING_VALUES_DYN)
                SelectNextVariable = SelectNextVariableMRV;
            if (heur == VariableHeuristics.MOST_CONSTRAINED)
            {
                RemoveInitiallyBlockedValuesFromDomains();
                Variables = Variables.OrderBy(v => v.Domain.Count).ToList();
                foreach (SudokuVariable v in Variables)
                    v.ResetDomain();
            }


        }

        public void ApplyValueHeuristics(ValueHeuristics heur)
        {
            valHeur = heur;
            if (heur == ValueHeuristics.DEFINITION_ORDER)
                return;
            if (heur == ValueHeuristics.RANDOM)
            {
                Random rand = new Random(2);
                foreach (SudokuVariable variable in Variables)
                {
                    variable.OrderedDomain = variable.Domain.OrderBy(v => rand.Next()).ToList();
                    variable.ResetDomain();
                }
            }
            if (heur == ValueHeuristics.LEAST_CONSTRAINING_VALUE)
                foreach (SudokuVariable v in Variables)
                    if(v.IsEmpty())
                        OrderDomain(v);
        }

        private void OrderDomain(SudokuVariable currElem)
        {
            int removedValues;
            Dictionary<int, int> valuesRemovedByEachNumber = new Dictionary<int, int>();
            foreach (int value in currElem.Domain)
            {
                removedValues = int.MaxValue;
                currElem.Value = value;
                if (CheckConstraints(currElem))
                {
                    removedValues = FilterDomainsOfAllRelatedFields(currElem);
                    currElem.Value = 0;
                    ResetDomainsOfAllRelatedFields(currElem);
                }
                else
                    currElem.Value = 0;
                valuesRemovedByEachNumber.Add(value, removedValues);
            }
            currElem.OrderedDomain = valuesRemovedByEachNumber.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
            currElem.ResetDomain();
        }

        private void RemoveInitiallyBlockedValuesFromDomains()
        {
            foreach (SudokuVariable variable in Variables)
            {
                if (variable.IsEmpty())
                    FilterDomain(variable);
            }
        }

        private int FilterDomainsOfAllRelatedFields(SudokuVariable variable)
        {
            int removedValues = 0;
            foreach (SudokuVariable related in relatedFields[variable])
                if(related.IsEmpty())
                    removedValues += FilterDomain(related);
            return removedValues;
        }

        private int FilterDomain(SudokuVariable variable)
        {
            variable.ResetDomain();
            int removedValues = 0;
            foreach (SudokuVariable related in relatedFields[variable])
                if (!related.IsEmpty())
                {
                    bool removed = variable.Domain.Remove(related.Value);
                        if(removed)
                            removedValues++;
                }

            return removedValues;
        }

        private void ResetDomainsOfAllRelatedFields(SudokuVariable variable)
        {
            foreach (SudokuVariable related in relatedFields[variable])
                related.ResetDomain();
        }

        public void SaveResult()
        {
            string FILE_NAME = $"C://Users//pkost//Workspaces//dotnet//SI_CSP//SI_CSP//Data//Results//Sudoku//" +
                $"Sudoku_{DateTime.Now.Day}_{DateTime.Now.Hour}_{BackTrack}_{varHeur}_{valHeur}.csv";

            if (!File.Exists(FILE_NAME))
            {
                File.AppendAllText(FILE_NAME, "Instancja, Difficulty, Czas znalezienia pierwszego rozwiazania, Liczba odwiedzonych wezlow do znalezienia pierwszego rozwiazania," +
                    "Liczba nawrotow do znalezienia pierwszego rozwiazania, Calkowity czas dzialania metody, Calkowita liczba odwiedzonych wezlow," +
                    "Calkowita liczba nawrotow, Liczba rozwiazan\n");
            }
            File.AppendAllText(FILE_NAME, $"{InstanceID}, {Difficulty}, {FirstSolutionTime},{NumOfNodesFirstSolution}," +
                $"{NumOfReturnsFirstSolution}, {TotalTime}, {TotalNodesVisited}," +
                $"{TotalReturns}, {Solutions.Count}\n");
        }
    }
}
