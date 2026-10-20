using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class NakedSingleTechnique : TechniqueBase<NakedSingleTechnique.IteratorImpl, NakedSingleTechnique.SolverImpl>
{
    public override SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Iterate(sudoku);
        var query =
            from item in items
            let solveStep = Solver.Solve(item)
            where solveStep is not null
            select solveStep;

        return query.FirstOrDefault();
    }

    public record Iteration(Cell Cell);

    public class IteratorImpl
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

    public class SolverImpl
    {
        public SolveStep? Solve(Iteration iteration)
        {
            var candidates = iteration.Cell.GetCurrentCandidateValues();
            if (candidates.Length == 1)
            {
                var candidateValue = candidates[0];
                return new SolveStep(
                [
                    new SolveStepItem(
                        [iteration.Cell.Id],
                        candidateValue,
                        SolveStepItemType.CandidateConfirmation,
                        "Naked Single",
                        $"The cell {iteration.Cell.Name} only has a single candidate left, value {Candidate.PrintValue(candidateValue)}.")
                ]);
            }

            return null;
        }
    }
}