using System;
using System.Collections.Generic;
using System.Text;

namespace SI_CSP.Problems.Jolka
{
    public class JolkaGrid
    {
        const char BLOCKED = '#';
        public char[][] Fields { get; set; }
        public int NumOfRows { get; private set; }
        public int NumOfCols { get; private set; }

        public JolkaGrid(char[][] grid)
        {
            Fields = grid;
            NumOfRows = grid.Length;
            NumOfCols = grid[0].Length;
        }

        public List<Tuple<int, int>> GetSharedFields()
        {
            List<Tuple<int, int>> SharedFields = new List<Tuple<int, int>>();
            for (int i = 0; i < NumOfRows; i++)
            {
                for (int j = 0; j < NumOfCols; j++)
                {
                    if (Fields[i][j] != BLOCKED) //znalezione wolne pole
                    {
                        if (j == 0) //jest w lewej kolumnie
                            if (Fields[i][j + 1] != BLOCKED) //po prawej ma wolne
                                if (IsFieldSharedVertically(i, j))
                                    SharedFields.Add(new Tuple<int, int>(i, j));

                        if (j == NumOfCols - 1) //jest w prawej kolumnie
                            if (Fields[i][j - 1] != BLOCKED) //po lewej ma wolne
                                if (IsFieldSharedVertically(i, j))
                                    SharedFields.Add(new Tuple<int, int>(i, j));
                        if (j < NumOfCols - 1 && j > 0) //jest w srodku kolumny
                            if (Fields[i][j + 1] != BLOCKED || Fields[i][j - 1] != BLOCKED) //ma wolne po prawej lub lewej
                                if (IsFieldSharedVertically(i, j))
                                    SharedFields.Add(new Tuple<int, int>(i, j));

                    }
                }
            }
            return SharedFields;
        }

        private bool IsFieldSharedVertically(int i, int j)
        {
            if (i == 0)//jest to gorny wiersz
            {
                if (Fields[i + 1][j] != BLOCKED) //i na dole ma wolne
                    return true;
            }
            else if (i == NumOfRows - 1) //jest to dolny wiersz
            {
                if (Fields[i - 1][j] != BLOCKED) //i na gorze ma wolne
                    return true;
            }
            else if (Fields[i - 1][j] != BLOCKED || Fields[i + 1][j] != BLOCKED) // na gorze lub na dole ma wolne
                return true;

            return false;
        }

        public bool IsBlocked(int i, int j)
        {
            return Fields[i][j] == BLOCKED;
        }

        public void WriteVariable(JolkaVariable variable)
        {
            if (variable.Horizontal)
                for (int i = 0; i < variable.Word.Length; i++)
                {
                    Fields[variable.SectionNum][variable.BeginIndex + i] = variable.Word[i];
                }
            else
                for (int i = 0; i < variable.Word.Length; i++)
                {
                    Fields[variable.BeginIndex + i][variable.SectionNum] = variable.Word[i];
                }
        }

        internal void PrintGrid()
        {
            foreach (char[] row in Fields)
                Console.WriteLine(new string(row));
            Console.WriteLine();
        }
    }
}
