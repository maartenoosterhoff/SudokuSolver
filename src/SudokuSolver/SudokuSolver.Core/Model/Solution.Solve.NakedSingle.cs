using System.Linq;

namespace SudokuSolver.Core
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding a Naked Single.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryNakedSingle()
        {
            var step = new SolutionStep(false);

            var solutions = from i in Enumerable.Range(0, Dimension)
                            let cell = _cellList[i]
                            where cell.Value == Candidate.NotSet
                            where cell.CandidateCount() == 1
                            from c in Enumerable.Range(0, Candidate.PossibleCandidateCount)
                            where cell.Candidates[c]
                            select new { cell, i, c };
            var solution = solutions.FirstOrDefault();
            if (solution != null)
            {
                SetCell(solution.i, solution.c);
                step = new SolutionStep(true, "Found a Naked Single in cell " + solution.cell.Name + " - Setting cell to value " + Candidate.PrintValue(solution.c) + "\r\n");
            }
                            


            //for (var i = 0; i < Dimension && !step.StepTaken; i++) {
            //    var cell = _cellList[i];
            //    if (cell.Value == -1) {
            //        if (cell.CandidateCount() == 1) {
            //            for (var c = 0; c < Candidate.PossibleCandidateCount; c++) {
            //                if (cell.Candidates[c]) {
            //                    SetCell(i, c);
            //                    step = new SolutionStep(true, "Found a Naked Single in cell " + cell.Name + " - Setting cell to value " + Candidate.printValue(c) + "\r\n");
            //                }
            //            }
            //        }
            //    }
            //}

            return step;
        }
    }
}
