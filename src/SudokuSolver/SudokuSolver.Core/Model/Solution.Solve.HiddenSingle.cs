using System.Linq;

namespace SudokuSolver.Core
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding a Hidden Single.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryHiddenSingle()
        {
            var step = new SolutionStep(false);
            var solutions = from candidateValue in Enumerable.Range(0, Candidate.PossibleCandidateCount)
                            from groupObject in _groupList
                            let applicableCells = groupObject.Cells.Where(c => _cellList[c].Candidates[candidateValue]).ToArray()
                            where applicableCells.Length == 1
                            select new { lastCell = applicableCells.First(), candidateValue, groupObject };

            var solution = solutions.FirstOrDefault();
            if (solution != null)
            {
                SetCell(solution.lastCell, solution.candidateValue);
                step = new SolutionStep(true, "Found a Hidden Single in group " + solution.groupObject.Name + " - Setting cell " + _cellList[solution.lastCell].Name + " to value " + Candidate.PrintValue(solution.candidateValue) + "\r\n");
            }

            return step;


            //var cellCount = 0;
            //var lastCell = Candidate.NotSet;
            //
            //for (var v = 0; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
            //    for (var g = 0; g < _groupList.Count && !step.StepTaken; g++) {
            //        cellCount = 0;
            //        foreach (var c in _groupList[g].Cells) {
            //            if (_cellList[c].Candidates[v]) {
            //                cellCount++;
            //                lastCell = c;
            //            }
            //        }
            //        if (cellCount == 1) {
            //            SetCell(lastCell, v);
            //            step = new SolutionStep(true, "Found a Hidden Single in group " + _groupList[g].Name + " - Setting cell " + _cellList[lastCell].Name + " to value " + Candidate.printValue(v) + "\r\n");
            //        }
            //    }
            //}

            //return step;
        }
    }
}
