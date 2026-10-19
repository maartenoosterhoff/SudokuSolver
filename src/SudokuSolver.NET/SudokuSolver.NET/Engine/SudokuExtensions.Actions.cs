using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine;

internal static partial class SudokuExtensions
{
    extension(Sudoku sudoku)
    {
        public void Reset()
        {
            sudoku.State = SudokuState.Reset;
            foreach (var cell in sudoku.Cells)
            {
                cell.Reset();
            }

            sudoku.GetCandidateBitSetCache().Clear();
        }

        public void SetCell(int cellId, int value)
        {
            sudoku.Cells[cellId].SetValue(value);

            //OnCellValueSet(cellId, value);

            var cellsWithCandidatesToUnset = from @group in sudoku.GroupCells[sudoku.Cells[cellId]]
                                             from cell in @group.Cells
                                             where cell.Id != cellId
                                             select cell;
            foreach (var cell in cellsWithCandidatesToUnset)
            {
                sudoku.RemoveCandidate(cell, value);
            }

            // Clear cache completely
            sudoku.GetCandidateBitSetCache().Clear();
        }

        public void RemoveCandidate(int cellId, int candidate)
        {
            if (!sudoku.Cells[cellId].Candidates[candidate])
            {
                return;
            }

            sudoku.Cells[cellId].RemoveCandidate(candidate);

            // Remove cache-item for this candidate.
            sudoku.GetCandidateBitSetCache().Remove(candidate, out _);
        }

        public void RemoveCandidate(Cell cell, int candidate)
        {
            if (!cell.Candidates[candidate])
            {
                return;
            }

            cell.RemoveCandidate(candidate);

            // Remove cache-item for this candidate.
            sudoku.GetCandidateBitSetCache().Remove(candidate, out _);
        }
    }
}