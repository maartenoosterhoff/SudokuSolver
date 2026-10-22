using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class NakedSingleTechnique : TechniqueBase<NakedSingleTechnique.IteratorImpl, NakedSingleTechnique.SolverImpl, NakedSingleTechnique.Iteration>
{
    public sealed record Iteration(Cell Cell);

    public sealed class IteratorImpl : IIterator<Iteration>
    {
        public IEnumerable<Iteration> Iterate(Sudoku sudoku)
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

    public sealed class SolverImpl : ISolver<Iteration>
    {
        public SolveStep? Solve(Sudoku sudoku, Iteration iteration)
        {
            var candidates = iteration.Cell.GetCurrentCandidateValues();
            if (candidates.Length != 1)
            {
                return null;
            }

            return new SolveStep(
            [
                new SolveStepItem(
                    [iteration.Cell.Id],
                    candidates[0],
                    SolveStepItemType.CandidateConfirmation,
                    "Naked Single",
                    $"The cell {iteration.Cell.Name} only has a single candidate left, value {Candidate.PrintValue(candidates[0])}.")
            ]);

        }
    }
}