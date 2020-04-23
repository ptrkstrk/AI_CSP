using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SI_CSP.Problems.Jolka
{
    public class Jolka : ProblemDiagnostics, IProblem
    {
        public JolkaGrid Grid { get; set; }
        public HashSet<string> Words;
        //public HashSet<string> UsedWords = new HashSet<string>();

        //kluczem jest zmienna, a wartoscia wszystkie ograniczenia dotyczace tej zmiennej
        public Dictionary<JolkaVariable, List<JolkaConstraint>> Constraints { get; set; } = 
            new Dictionary<JolkaVariable,List<JolkaConstraint>>();
        private Func<JolkaVariable> SelectNextVariable;

        public int NumOfRows;
        public int NumOfCols;
        public List<JolkaVariable> Variables { get; set; } = new List<JolkaVariable>();
        public List<List<JolkaVariable>> Solutions { get; set; } = new List<List<JolkaVariable>>();
        public bool StaticValHeur { get; private set; }
        public bool BackTrack { get; private set; } = false;
        VariableHeuristics varHeur;
        ValueHeuristics valHeur;
        public int InstanceID;

        public Jolka(char[][] grid, HashSet<string> words, int id)
        {
            InstanceID = id;
            Grid = new JolkaGrid(grid);
            Words = words;
            NumOfRows = grid.Length;
            NumOfCols = grid[0].Length;
            SelectNextVariable = SelectNextFreeVariable;
            InitVariables();
            InitConstraints();
            ApplyVariableHeuristics(VariableHeuristics.MIN_REMAINING_VALUES_DYN);
            ApplyValueHeuristics(ValueHeuristics.LEAST_CONSTRAINING_VALUE);
        }

        public void RunBacktracking()
        {
            if (firstRun)
            {
                BackTrack = true;
                StartTiming();
            }

            JolkaVariable currElem = SelectNextVariable();
            if (currElem == null) //znaleziono rozwiązanie
            {
                if (Solutions.Count == 0)
                    SaveFirstSolutionData();

                Solutions.Add(Variables.Select(v => new JolkaVariable(v)).ToList());
            }
            else
            {
                foreach (string value in currElem.Domain)//.Except(UsedWords))
                {
                    currElem.Word = value;
                    TotalNodesVisited++;
                    bool correct = CheckConstraints(currElem);
                    if (correct)
                        RunBacktracking();
                }
                currElem.Word = "";
            }
            TotalReturns++;//Nawrót - brak kolejnej wartosci
            TotalTime = stopwatch.Elapsed.TotalSeconds;
        }

        public void RunForwardChecking()
        {
            if (firstRun)
                StartTiming();

            JolkaVariable currElem = SelectNextVariable();
            if (currElem == null) //znaleziono rozwiązanie
            {
                if (Solutions.Count == 0)
                    SaveFirstSolutionData();

                Solutions.Add(Variables.Select(v => new JolkaVariable(v)).ToList());
            }
            else
            {
                foreach (string value in currElem.Domain)//.Except(UsedWords))
                {
                    currElem.Word = value;
                    TotalNodesVisited++;
                    FilterDomainsOfAllRelatedVariables(currElem);
                    if (!IsAnyDomainEmpty())
                        RunForwardChecking();
                }
                currElem.Word = ""; //Nawrót - brak kolejnej wartosci
                FilterDomainsOfAllRelatedVariables(currElem);
            }
            TotalReturns++;
            TotalTime = stopwatch.Elapsed.TotalSeconds;
        }


        private void ResetDomainsOfAllRelatedVariables(JolkaVariable currElem)
        {
            JolkaVariable related;
            foreach (JolkaConstraint cons in Constraints[currElem])
            {
                if (currElem.Horizontal)
                    related = cons.VerticalVariable;
                else
                    related = cons.HorizontalVariable;
                related.ResetDomain();//Domain = GetWordsWithLength(related.EndIndex - related.BeginIndex + 1);
            }
        }
        

        private int FilterDomainsOfAllRelatedVariables(JolkaVariable currElem)
        {
            JolkaVariable related;
            int valuesRemoved = 0;
            foreach (JolkaConstraint cons in Constraints[currElem])
            {
                if (currElem.Horizontal)
                    related = cons.VerticalVariable;
                else
                    related = cons.HorizontalVariable;
                if (related.IsEmpty())
                    valuesRemoved += filterDomain(related);
            }

            return valuesRemoved;
        }

        private int filterDomain(JolkaVariable variable)
        {
            int valuesRemoved = 0;
            variable.ResetDomain();//Domain = GetWordsWithLength(variable.EndIndex - variable.BeginIndex + 1);

            if (variable.Horizontal)
                foreach (JolkaConstraint cons in Constraints[variable])
                {
                    if (!cons.VerticalVariable.IsEmpty())
                        valuesRemoved += variable.Domain.RemoveAll(word => word[cons.PosHorizontal] != cons.VerticalVariable.Word[cons.PosVertical]);
                }
            else
                foreach (JolkaConstraint cons in Constraints[variable])
                {
                    if (!cons.HorizontalVariable.IsEmpty())
                        valuesRemoved += variable.Domain.RemoveAll(word => word[cons.PosVertical] != cons.HorizontalVariable.Word[cons.PosHorizontal]);
                }
            return valuesRemoved;

        }

        private bool IsAnyDomainEmpty()
        {
            return Variables.Any(v => v.Domain.Count == 0);
        }

        private bool CheckConstraints(JolkaVariable currElem)
        {
            foreach (JolkaConstraint constraint in Constraints[currElem])
                if (!constraint.isMet())
                    return false;

            return true;
        }

        public JolkaVariable SelectNextFreeVariable()
        {
            return Variables.FirstOrDefault(v=>v.Word == "");
        }

        public void InitVariables()
        {
            initHorizontal();
            initVertical();
        }

        private void initVertical()
        {
            int beginIndex = 0;
            int endIndex;
            for (int i = 0; i < NumOfCols; i++)
            {
                bool started = false;
                for (int j = 0; j < NumOfRows; j++)
                {
                    if (!started && !Grid.IsBlocked(j, i))
                    {
                        started = true;
                        beginIndex = j;
                    }
                    if (started && (Grid.IsBlocked(j, i) || j == NumOfRows - 1))
                    {
                        endIndex = Grid.IsBlocked(j, i) ? j - 1 : j;
                        if (endIndex != beginIndex)
                            Variables.Add(new JolkaVariable()
                            {
                                Horizontal = false,
                                SectionNum = i,
                                BeginIndex = beginIndex,
                                EndIndex = endIndex,
                                OrderedDomain = GetWordsWithLength(endIndex - beginIndex + 1),
                                Domain = GetWordsWithLength(endIndex - beginIndex + 1)
                            });
                        started = false;
                    }
                }
            }
        }

        private void initHorizontal()
        {
            int beginIndex = 0;
            int endIndex;
            for (int i = 0; i < NumOfRows; i++)
            {
                bool started = false;
                for (int j = 0; j < NumOfCols; j++)
                {
                    if (!started && !Grid.IsBlocked(i, j))
                    {
                        started = true;
                        beginIndex = j;
                    }
                    if (started && (Grid.IsBlocked(i, j) || j == NumOfCols-1))
                    {
                        endIndex = Grid.IsBlocked(i, j) ? j - 1 : j;
                        if (endIndex != beginIndex)
                            Variables.Add(new JolkaVariable()
                            {
                                Horizontal = true,
                                SectionNum = i,
                                BeginIndex = beginIndex,
                                EndIndex = endIndex,
                                OrderedDomain = GetWordsWithLength(endIndex - beginIndex + 1),
                                Domain = GetWordsWithLength(endIndex - beginIndex + 1)
                            });
                        started = false;
                    }

                }
            }
        }

        private void InitConstraints()
        {
            foreach (JolkaVariable variable in Variables)
                Constraints.Add(variable, new List<JolkaConstraint>());

            List<Tuple<int, int>> SharedFields = Grid.GetSharedFields();
            foreach (Tuple<int, int> field in SharedFields)
                AddConstraint(field.Item1, field.Item2);
        }

        private void AddConstraint(int i, int j)
        {
            JolkaVariable horizontalVar = Variables.
                First(v => (v.Horizontal && v.SectionNum == i && v.BeginIndex <= j && v.EndIndex >= j));
            JolkaVariable verticalVar = Variables.
                First(v => (!v.Horizontal && v.SectionNum == j && v.BeginIndex <= i && v.EndIndex >= i));
            
            JolkaConstraint constraint = new JolkaConstraint(ref horizontalVar, ref verticalVar, i, j);
            Constraints[horizontalVar].Add(constraint);
            Constraints[verticalVar].Add(constraint);
        }

        private List<string> GetWordsWithLength(int length)
        {
            return Words.Where(w => w.Length == length).ToList();
        }

        public void ApplyVariableHeuristics(VariableHeuristics heur)
        {
            varHeur = heur;
            if (heur == VariableHeuristics.DEFINITION_ORDER)
                return;
            if (heur == VariableHeuristics.RANDOM)
            {
                Random rand = new Random(100);
                Variables = Variables.OrderBy(v => rand.Next()).ToList();
            }
            if (heur == VariableHeuristics.MOST_CONSTRAINED)
                Variables = Variables.OrderByDescending(v => Constraints[v].Count).ToList();
            if (heur == VariableHeuristics.ROWS_DESCENDING)
                Variables = Variables.OrderBy(v => v.Horizontal ? v.SectionNum * 1000 + v.BeginIndex : v.SectionNum + v.BeginIndex * 1000).ToList();
            if (heur == VariableHeuristics.FROM_TOP_LEFT_CORNER)
                Variables = Variables.OrderBy(v => v.SectionNum + v.BeginIndex).ToList();
            if (heur == VariableHeuristics.FROM_TOP_LEFT_LEAST_DOMAIN)
                Variables = Variables.OrderBy(v => v.Domain.Count).OrderBy(v => v.SectionNum + v.BeginIndex).ToList();
            if (heur == VariableHeuristics.LEAST_DOMAIN)
                Variables = Variables.OrderBy(v => v.Domain.Count).ToList();
            
            if (heur == VariableHeuristics.MIN_REMAINING_VALUES_DYN)
                SelectNextVariable = SelectNextVariableMRV;
        }

        public void ApplyValueHeuristics(ValueHeuristics heur)
        {
            valHeur = heur;
            if (heur == ValueHeuristics.DEFINITION_ORDER)
                return;
            if (heur == ValueHeuristics.RANDOM)
            {
                Random rand = new Random(100);
                foreach (JolkaVariable variable in Variables)
                {
                    variable.OrderedDomain = variable.OrderedDomain.OrderBy(v => rand.Next()).ToList();
                    variable.ResetDomain();
                }
            }
            if (heur == ValueHeuristics.LEAST_CONSTRAINING_VALUE)
            {
                foreach (JolkaVariable variable in Variables)
                {
                    OrderDomain(variable);
                }
            }
        }

        private JolkaVariable SelectNextVariableMRV()
        {
            if (BackTrack)
            {
                JolkaVariable nextVar = null;
                int minRemainingValues = int.MaxValue;
                foreach (JolkaVariable variable in Variables)
                {
                    if (variable.IsEmpty())
                    {
                        int excludedValuesCount = 0;
                        foreach (JolkaConstraint cons in Constraints[variable])
                        {
                            JolkaVariable related;
                            if (variable.Horizontal)
                                related = cons.VerticalVariable;
                            else
                                related = cons.HorizontalVariable;

                            if (!related.IsEmpty())
                            {
                                foreach (string word in variable.Domain)
                                {
                                    variable.Word = word;
                                    if (!cons.isMet())
                                        excludedValuesCount++;
                                }
                            }
                            if (variable.Domain.Count - excludedValuesCount < minRemainingValues)
                            {
                                minRemainingValues = variable.Domain.Count - excludedValuesCount;
                                nextVar = variable;
                            }
                            variable.Word = "";
                        }
                    }
                }
                return nextVar;
            }
            else
                return Variables.OrderBy(v => v.Domain.Count).FirstOrDefault(v => v.IsEmpty());
        }

        private void OrderDomain(JolkaVariable variable)
        {
            int valuesRemoved = 0;
            Dictionary<string, int> valuesRemovedByEachWord = new Dictionary<string, int>();
            foreach (string word in variable.Domain)
            {
                variable.Word = word;
                valuesRemoved = FilterDomainsOfAllRelatedVariables(variable);
                variable.Word = "";
                ResetDomainsOfAllRelatedVariables(variable);
                //if (Variables.Any(v => v.Word == word))
                //    valuesRemoved += 1000;
                valuesRemovedByEachWord.Add(word, valuesRemoved);
            }
            variable.OrderedDomain = variable.OrderedDomain.OrderBy(w => valuesRemovedByEachWord[w]).ToList();
            variable.ResetDomain();
        }

        public void PresentSolutions()
        {
            foreach (List<JolkaVariable> solution in Solutions)
            {
                foreach (JolkaVariable variable in solution)
                    Grid.WriteVariable(variable);
                
                Grid.PrintGrid();
            }
        }

        public void PresentPerformance()
        {
            base.PresentPerformance(Solutions.Count);
        }

        public void SaveResult()
        {
            string FILE_NAME = $"C://Users//pkost//Workspaces//dotnet//SI_CSP//SI_CSP//Data//Results//Jolka//" +
                $"Jolka_{DateTime.Now.Day}_{DateTime.Now.Hour}_{BackTrack}_{varHeur}_{valHeur}.csv";

            if (!File.Exists(FILE_NAME))
            {
                File.AppendAllText(FILE_NAME, "Instancja, Czas znalezienia pierwszego rozwiazania, Liczba odwiedzonych wezlow do znalezienia pierwszego rozwiazania," +
                    "Liczba nawrotow do znalezienia pierwszego rozwiazania, Calkowity czas dzialania metody, Calkowita liczba odwiedzonych wezlow," +
                    "Calkowita liczba nawrotow, Liczba rozwiazan\n");
            }
            File.AppendAllText(FILE_NAME, $"{InstanceID}, {FirstSolutionTime},{NumOfNodesFirstSolution}," +
                $"{NumOfReturnsFirstSolution}, {TotalTime}, {TotalNodesVisited}," +
                $"{TotalReturns}, {Solutions.Count}\n");
        }

        //private void RemoveBlockedWordsFromDomains(JolkaVariable currElem)
        //{
        //    JolkaVariable related;
        //    foreach (JolkaConstraint cons in Constraints[currElem])
        //    {
        //        if (currElem.Horizontal)
        //            related = cons.VerticalVariable;
        //        else
        //            related = cons.HorizontalVariable;
        //        if (related.IsEmpty())
        //        {
        //            List<string> deletedWords = new List<string>();
        //            foreach (string word in related.Domain)
        //            {
        //                related.Word = word;
        //                if (!cons.isMet())
        //                {
        //                    deletedWords.Add(word);
        //                }
        //            }
        //            related.Word = "";
        //            foreach (string word in deletedWords)
        //                related.Domain.Remove(word);
        //        }
        //    }
        //}

        //private int CountConstraintsViolations(JolkaVariable currElem)
        //{
        //    int violations = 0;
        //    foreach (JolkaConstraint constraint in Constraints[currElem])
        //        if (!constraint.isMet())
        //            violations++;

        //    return violations;
        //}
    }
}