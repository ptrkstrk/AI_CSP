using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SI_CSP.Problems.Sudoku
{
    public class SudokuVariable : IVariable
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Value { get; set; } = 0;
        public HashSet<int> Domain { get; set; }
        public List<int> OrderedDomain { get; set; }

        public SudokuVariable(int row, int col, int value)
        {
            Row = row;
            Col = col;
            Value = value;
            OrderedDomain = Enumerable.Range(1, 9).ToList();
            if (value != 0)
                Domain = new HashSet<int> { value };
            else
                ResetDomain();
        }

        public SudokuVariable(SudokuVariable other)
        {
            Row = other.Row;
            Col = other.Col;
            Value = other.Value;
            Domain = other.Domain;
            OrderedDomain = other.OrderedDomain;
        }

        public bool IsEmpty()
        {
            return Value == 0;
        }

        public void ResetDomain()
        {
            Domain = new HashSet<int>(OrderedDomain);
        }
    }
}
