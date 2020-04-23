using System;
using System.Collections.Generic;
using System.Text;

namespace SI_CSP.Problems.Jolka
{
    public class JolkaVariable : IVariable
    {
        public string Word { get; set; } = "";
        public bool Horizontal { get; set; }
        public int SectionNum { get; set; }
        public int BeginIndex, EndIndex;
        public List<string> Domain { get; set; }
        public List<string> OrderedDomain { get; set; }

        public JolkaVariable() { }
        public JolkaVariable(JolkaVariable other)
        {
            Word = other.Word;
            Horizontal = other.Horizontal;
            SectionNum = other.SectionNum;
            BeginIndex = other.BeginIndex;
            EndIndex = other.EndIndex;
            Domain = other.Domain;
            OrderedDomain = other.OrderedDomain;
        }

        public bool IsEmpty()
        {
            return Word == "";
        }

        public void ResetDomain()
        {
            Domain = new List<string>(OrderedDomain);
        }

    }
}
