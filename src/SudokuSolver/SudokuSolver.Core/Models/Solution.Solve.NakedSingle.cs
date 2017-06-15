using System.Linq;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        private SolutionStep TryNakedSingle()
        {
            var step = new SolutionStep(false);

            var solutions = from i in Enumerable.Range(0, Dimension)
                            let cell = _cellList[i]
                            where cell.Value == Candidate.NotSet
                            where cell.CurrentCandidateCount() == 1
                            from c in Enumerable.Range(0, Candidate.PossibleCandidateCount)
                            where cell.Candidates[c]
                            select new { cell, i, c };
            var solution = solutions.FirstOrDefault();
            if (solution != null)
            {
                SetCell(solution.i, solution.c);
                step = new SolutionStep(true, "Found a Naked Single in cell " + solution.cell.Name + " - Setting cell to value " + Candidate.PrintValue(solution.c) + "\r\n");
            }

            return step;
        }
    }
}
