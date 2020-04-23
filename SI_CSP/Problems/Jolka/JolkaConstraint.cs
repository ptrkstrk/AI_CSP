using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SI_CSP.Problems.Jolka
{
    public class JolkaConstraint
    {
        public JolkaVariable HorizontalVariable { get; set; }
        public JolkaVariable VerticalVariable { get; set; }
        public int PosVertical { get; set; }
        public int PosHorizontal { get; set; }

        public JolkaConstraint(ref JolkaVariable horizontalVar, ref JolkaVariable verticalVar, 
            int row, int col)
        {
            HorizontalVariable = horizontalVar;
            VerticalVariable = verticalVar;
            PosVertical = row - VerticalVariable.BeginIndex;
            PosHorizontal = col - HorizontalVariable.BeginIndex;
        }

        public bool isMet()
        {
            if (HorizontalVariable.Word != "" && VerticalVariable.Word != "")
                return HorizontalVariable.Word[PosHorizontal]
                    == VerticalVariable.Word[PosVertical];
            return true;
        }
    }
}
