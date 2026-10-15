using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class NakedSingleTechnique : ITechnique
{
    public SolveStep? Solve(Sudoku sudoku)
    {
        var solutions =
            // Try all cells on the board
            from cell in sudoku.Cells
            // Where the cell doesn't contain a value yet
            where cell.Value == Candidate.NotSet
            // Take only cells which have a single candidate left
            where cell.CurrentCandidateCount() == 1
            // Loop through all candidates
            from candidateValue in Enumerable.Range(0, sudoku.CandidateCount)
                // Select the candidate that is left
            where cell.Candidates[candidateValue]
            // Yes! Take the candidate for that cell, and place the candidate
            select new SolveStep(
            [
                new SolveStepItem(
                    [cell.Id],
                    candidateValue,
                    SolveStepItemType.CandidateConfirmation,
                    "Naked Single",
                    $"The cell {cell.Name} only has a single candidate left, value {Candidate.PrintValue(candidateValue)}.")
            ]);

        return solutions.FirstOrDefault();
    }
}