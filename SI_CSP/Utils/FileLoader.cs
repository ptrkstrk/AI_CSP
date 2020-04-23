using SI_CSP.Problems;
using SI_CSP.Problems.Jolka;
using SI_CSP.Problems.Sudoku;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SI_CSP.Utils
{
    class FileLoader
    {
        private const string SUDOKU_FILE =
            "C:/Users/pkost/Workspaces/dotnet/SI_CSP/SI_CSP/Data/ai-lab2-2020-dane/Sudoku.csv";
        
        public static List<Sudoku> LoadSudoku()
        {
            List<string> rows = File.ReadAllLines(SUDOKU_FILE).ToList();
            rows.RemoveAt(0);
            //rows.RemoveAt(rows.Count - 1);

            List<Sudoku> sudokus = new List<Sudoku>();

            foreach (string row in rows)
            {
                string[] rowElems = row.Split(';');
                int instance;
                int.TryParse(rowElems[0], out instance);
                float difficulty;
                float.TryParse(rowElems[1], out difficulty);
                string boardElems = rowElems[2];
                SudokuVariable[][] board = new SudokuVariable[9] [];
                for (int i = 0; i < 9; i++)
                    board[i] = new SudokuVariable[9];
                int currElem;
                for (int i = 0; i < 81; i++)
                {
                    if (int.TryParse(boardElems[i].ToString(), out currElem))
                        board[i / 9][i % 9] = new SudokuVariable(i / 9, i % 9, currElem);
                    else
                        board[i / 9][i % 9] = new SudokuVariable(i / 9, i % 9, 0);
                }
                sudokus.Add(new Sudoku(board, instance, difficulty));
            }

            return sudokus;
        }

        public static Jolka LoadJolka(int num)
        {
            HashSet<string> words = File.ReadAllLines(
                $"C:/Users/pkost/Workspaces/dotnet/SI_CSP/SI_CSP/Data/ai-lab2-2020-dane/Jolka/words{num}").
                ToHashSet();
            List<string> puzzleRows = File.ReadAllLines(
                $"C:/Users/pkost/Workspaces/dotnet/SI_CSP/SI_CSP/Data/ai-lab2-2020-dane/Jolka/puzzle{num}")
                .ToList();

            char[][] grid = new char[puzzleRows.Count][];
            for (int i = 0; i < puzzleRows.Count; i++)
            {
                grid[i] = puzzleRows[i].ToCharArray();
            }

            return new Jolka(grid, words, num);
        }
    }
}
