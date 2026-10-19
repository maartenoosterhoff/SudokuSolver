using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class HiddenSingleTechnique : ITechnique
{
    public SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Get(sudoku);
        var query =
            from item in items
            let lastCell = Calculator.GetLastApplicableCell(sudoku, item)
            where lastCell is not null

            select new SolveStep(
            [
                new SolveStepItem(
                    [lastCell.Id],
                    item.CandidateValue,
                    SolveStepItemType.CandidateConfirmation,
                    "Hidden Single",
                    $"In group {item.Group.Name} the cell {lastCell.Name} is the only cell for candidate {Candidate.PrintValue(item.CandidateValue)}.")
            ]);

        return query.FirstOrDefault();
    }

    public record Iteration(Group Group, int CandidateValue);

    public static class Iterator
    {
        public static IEnumerable<Iteration> Get(Sudoku sudoku)
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

    public static class Calculator
    {
        public static Cell? GetLastApplicableCell(Sudoku sudoku, Iteration iteration)
        {
            // Deduce which cells in the group are available to place the candidate
            var applicableCells = iteration.Group.Cells.Where(c => c.Candidates[iteration.CandidateValue]).ToArray();
            // Make sure there is only a single cell in the group that is available
            if (applicableCells.Length == 1)
            {
                // Remember which cell it is
                return applicableCells[0];
            }

            return null;
        }
    }
}