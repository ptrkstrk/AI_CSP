using SI_CSP.Problems;
using SI_CSP.Problems.Jolka;
using SI_CSP.Problems.Sudoku;
using SI_CSP.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SI_CSP
{
    class Program
    {
        static void Main(string[] args)
        {
            //testSudokus();
            runJolkas();
            //runSudokus();
        }

        private static void testSudokus()
        {
            Stopwatch initSw = new Stopwatch();
            initSw.Start();
            List<Sudoku> allSudokus = FileLoader.LoadSudoku();
            List<Sudoku> sudokus = new List<Sudoku>();
            sudokus.Add(allSudokus.ElementAt(8));//diff 1
            sudokus.Add(allSudokus.ElementAt(20));//diff 4
            sudokus.Add(allSudokus.ElementAt(37));//diff 7
            sudokus.Add(allSudokus.ElementAt(41));//diff 8
            sudokus.Add(allSudokus.ElementAt(44));//diff 9

            Stopwatch allSw = new Stopwatch();
            allSw.Start();
            foreach (Sudoku sudoku in sudokus)
            {
                Console.WriteLine(sudoku.Difficulty);
                //sudoku.RunBacktracking();
                sudoku.RunForwardChecking();
                sudoku.SaveResult();
                //sudoku.PresentPerformance();
                //sudoku.PresentSolutions();
            }

            Console.WriteLine($"{allSw.Elapsed.TotalSeconds}");
            Console.WriteLine($"{allSw.Elapsed.TotalMilliseconds / sudokus.Count}");
        }

        private static void runJolkas()
        {

            List<Jolka> jolkas = new List<Jolka>();
            for (int i = 0; i < 5; i++)
            {
                jolkas.Add(FileLoader.LoadJolka(i));
            }

            Stopwatch allSw = new Stopwatch();
            allSw.Start();
            int totalVisited = 0;
            int totalReturns = 0;
            int firstVisited = 0;
            int firstReturns = 0;
            double firstTime = 0;
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"{i}: ");
                jolkas[i].RunForwardChecking();
                //jolkas[i].RunForwardChecking();
                totalVisited += jolkas[i].TotalNodesVisited;
                totalReturns += jolkas[i].TotalReturns;
                firstVisited += jolkas[i].NumOfNodesFirstSolution;
                firstReturns += jolkas[i].NumOfReturnsFirstSolution;
                firstTime += jolkas[i].FirstSolutionTime;
                //jolkas[i].SaveResult();
                jolkas[i].PresentPerformance();
                //totalVisited += jolkas[i].TotalNodesVisited;
                //jolkas[i].PresentSolutions();
            }
            Console.WriteLine($"{allSw.Elapsed.TotalSeconds}");
            //Console.WriteLine($"{allSw.Elapsed.TotalMilliseconds / sudokus.Count}");
            Console.WriteLine($"Visited Nodes: {totalVisited}");
            Console.WriteLine($"Returns: {totalReturns}");
            Console.WriteLine($"first Time: {firstTime}");
            Console.WriteLine($"first visited: {firstVisited}");
            Console.WriteLine($"first Returns: {firstReturns}");
        }

        private static void runSudokus()
        {
            Stopwatch initSw = new Stopwatch();
            initSw.Start();
            List<Sudoku> allSudokus = FileLoader.LoadSudoku();
            //Console.WriteLine($"{initSw.Elapsed.TotalSeconds}");
            List<Sudoku> sudokus = allSudokus.Where(s => s.Difficulty < 8).ToList();
            Stopwatch allSw = new Stopwatch();
            allSw.Start();
            int totalVisited = 0;
            int totalReturns = 0;
            int firstVisited = 0;
            int firstReturns = 0;
            double firstTime = 0;
            foreach (Sudoku sudoku in sudokus)
            {
                //Console.WriteLine(sudoku.InstanceID);
                //sudoku.RunBacktracking();
                //sudoku.SaveResult();
                sudoku.RunForwardChecking();
                totalVisited += sudoku.TotalNodesVisited;
                totalReturns += sudoku.TotalReturns;
                firstVisited += sudoku.NumOfNodesFirstSolution;
                firstReturns += sudoku.NumOfReturnsFirstSolution;
                firstTime += sudoku.FirstSolutionTime;
                //sudoku.PresentPerformance();
                //sudoku.PresentSolutions();
            }
            //sudokus[5].PresentCurrBoard();
            //sudokus[5].RunForwardChecking();
            //sudokus[5].RunBacktracking();
            //sudokus[5].PresentPerformance();
            //sudokus[5].SaveResult();
            //sudokus[5].PresentSolutions();

            Console.WriteLine($"{allSw.Elapsed.TotalSeconds}");
            //Console.WriteLine($"{allSw.Elapsed.TotalMilliseconds / sudokus.Count}");
            Console.WriteLine($"Visited Nodes: {totalVisited}");
            Console.WriteLine($"Returns: {totalReturns}");
            Console.WriteLine($"first Time: {firstTime}");
            Console.WriteLine($"first visited: {firstVisited}");
            Console.WriteLine($"first Returns: {firstReturns}");
        }
    }
}
