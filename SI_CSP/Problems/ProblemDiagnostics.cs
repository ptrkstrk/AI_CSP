using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SI_CSP.Problems
{
    public abstract class ProblemDiagnostics
    {
        protected bool firstRun = true;
        public Stopwatch stopwatch = new Stopwatch();
        public double FirstSolutionTime { get; set; }
        public int NumOfNodesFirstSolution { get; set; }
        public int NumOfReturnsFirstSolution { get; set; }
        public double TotalTime { get; set; }
        public int TotalNodesVisited { get; set; } = 0;
        public int TotalReturns { get; set; } = 0;

        public void PresentPerformance(int numOfSolutions)
        {
            Console.WriteLine($"Czas znalezienia pierwszego rozwiazania {FirstSolutionTime} s");
            Console.WriteLine(
                "Liczba odwiedzonych wezlow do znalezienia pierwszego rozwiazania: "
                + NumOfNodesFirstSolution);
            Console.WriteLine("Liczba nawrotów do znalezienia pierwszego rozwiązania: "
                + NumOfReturnsFirstSolution);
            Console.WriteLine("Całkowity czas działania metody: " + TotalTime + " s");
            Console.WriteLine("Całkowita liczba odwiedzonych węzłów: " + TotalNodesVisited);
            Console.WriteLine("Całkowita liczba nawrotów: " + TotalReturns);
            Console.WriteLine("Liczba rozwiązań: " + numOfSolutions);
            Console.WriteLine();
        }

        public void StartTiming()
        {
            stopwatch.Start();
            firstRun = false;
        }
        //public void RunBacktracking()
        //{

        //    //IVariable currElem = SelectNextVariable();
        //    //if (currElem == null) //znaleziono rozwiązanie
        //    //{
        //    //    if (Solutions.Count == 0)
        //    //    {
        //    //        NumOfNodesFirstSolution = TotalNodesVisited;
        //    //        FirstSolutionTime = stopwatch.Elapsed.TotalSeconds;
        //    //        NumOfReturnsFirstSolution = TotalReturns;
        //    //    }

        //    //    Solutions.Add(Variables.Select(v => new JolkaVariable(v)).ToList());
        //    //    //presentSolution();
        //    //    return;
        //    //}
        //    ////WriteVariableOnGrid(currElem);
        //    //foreach (string value in currElem.Domain)//.Except(UsedWords))
        //    //{
        //    //    currElem.Word = value;//currElem.Domain[i];
        //    //    TotalNodesVisited++;
        //    //    //UsedWords.Add(value);
        //    //    bool correct = CheckConstraints(currElem);
        //    //    if (correct)
        //    //        RunBacktracking();
        //    //    //UsedWords.Remove(value);
        //    //}
        //    ////eraseVariableFromGrid();
        //    //currElem.Word = "";
        //    //TotalReturns++;
        //    //TotalTime = stopwatch.Elapsed.TotalSeconds;
        //}

        internal void SaveFirstSolutionData()
        {
            NumOfNodesFirstSolution = TotalNodesVisited;
            FirstSolutionTime = stopwatch.Elapsed.TotalSeconds;
            NumOfReturnsFirstSolution = TotalReturns;
        }
    }
}
