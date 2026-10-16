using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class NakedSingleTechnique : ITechnique
{
    public SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Get(sudoku);
        var query =
            from item in items
            let candidateValue = Calculator.IsValid(item)
            where candidateValue is not null
            select new SolveStep(
            [
                new SolveStepItem(
                    [item.Cell.Id],
                    candidateValue.Value,
                    SolveStepItemType.CandidateConfirmation,
                    "Naked Single",
                    $"The cell {item.Cell.Name} only has a single candidate left, value {Candidate.PrintValue(candidateValue.Value)}.")
            ]);

        return query.FirstOrDefault();
    }

    public record Iteration(Cell Cell);

    public static class Iterator
    {
        public static IEnumerable<Iteration> Get(Sudoku sudoku)
        {
            return
                // Try all cells on the board
                from cell in sudoku.Cells
                // Where the cell doesn't contain a value yet
                where cell.Value == Candidate.NotSet
                // Select iteration values
                select new Iteration(cell);
        }
    }

    public static class Calculator
    {
        public static int? IsValid(Iteration iteration)
        {
            var candidates = iteration.Cell.GetCurrentCandidates();
            if (candidates.Length == 1)
            {
                return candidates[0];
            }

            return null;
        }
    }
}