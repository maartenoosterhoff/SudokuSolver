using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class HiddenSingleTechnique : TechniqueBase<HiddenSingleTechnique.IteratorImpl, HiddenSingleTechnique.SolverImpl>
{
    public override SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Iterate(sudoku);
        var query =
            from item in items
            let solveStep = Solver.Solve(sudoku, item)
            where solveStep is not null
            select solveStep;

        return query.FirstOrDefault();
    }

    public record Iteration(Group Group, int CandidateValue);

    public class IteratorImpl
    {
        public IEnumerable<Iteration> Iterate(Sudoku sudoku)
        {
            return
                // Try all candidates
                from candidateValue in Enumerable.Range(0, sudoku.CandidateCount)
                // Try every group in the sudoku
                from @group in sudoku.Groups
                // Where the group doesn't contain the candidate yet
                where @group.Cells.All(c => c.Value != candidateValue)
                // Select iteration values
                select new Iteration(@group, candidateValue);
        }
    }

    public class SolverImpl
    {
        public SolveStep? Solve(Sudoku sudoku, Iteration iteration)
        {
            // Deduce which cells in the group are available to place the candidate
            var applicableCells = iteration.Group.Cells.Where(c => c.Candidates[iteration.CandidateValue]).ToArray();
            // Make sure there is only a single cell in the group that is available
            if (applicableCells.Length == 1)
            {
                // Remember which cell it is
                var lastCell = applicableCells[0];

                return new SolveStep(
                [
                    new SolveStepItem(
                        [lastCell.Id],
                        iteration.CandidateValue,
                        SolveStepItemType.CandidateConfirmation,
                        "Hidden Single",
                        $"In group {iteration.Group.Name} the cell {lastCell.Name} is the only cell for candidate {Candidate.PrintValue(iteration.CandidateValue)}.")
                ]);
            }

            return null;
        }
    }
}