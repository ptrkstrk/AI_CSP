using System;
using System.Collections.Generic;
using System.Text;

namespace SI_CSP.Problems
{
    public interface IProblem
    {
        public void RunBacktracking();

        public void ApplyVariableHeuristics(VariableHeuristics heur);
        public void ApplyValueHeuristics(ValueHeuristics heur);
        public void PresentSolutions();
        public void PresentPerformance();
        public void InitVariables();
        //public IVariable SelectNextVariable();

        //public bool CheckConstraints(IVariable);
    }
}
