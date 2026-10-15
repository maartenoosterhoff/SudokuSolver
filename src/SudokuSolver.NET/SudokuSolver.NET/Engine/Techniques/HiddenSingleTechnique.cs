using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class HiddenSingleTechnique : ITechnique
{
    public SolveStep? Solve(Sudoku sudoku)
    {
        var solutions =
            // Try all candidates
            from candidateValue in Enumerable.Range(0, sudoku.CandidateCount)
            // Try every group in the sudoku
            from @group in sudoku.Groups
                // Where the group doesn't contain the candidate yet
            where @group.CellIds.All(c => sudoku.Cells[c].Value != candidateValue)
            // Deduce which cells in the group are available to place the candidate
            let applicableCells = @group.CellIds.Where(c => sudoku.Cells[c].Candidates[candidateValue]).ToArray()
            // Make sure there is only a single cell in the group that is available
            where applicableCells.Length == 1
            // Remember which cell it is
            let lastCell = sudoku.Cells[applicableCells.First()]
            // Yes! Take that cell in the group, and place the candidate
            select new SolveStep(
            [
                new SolveStepItem(
                    [lastCell.Id],
                    candidateValue,
                    SolveStepItemType.CandidateConfirmation,
                    "Hidden Single",
                    $"In group {@group.Name} the cell {lastCell.Name} is the only cell for candidate {Candidate.PrintValue(candidateValue)}.")
            ]);

        return solutions.FirstOrDefault();
    }
}