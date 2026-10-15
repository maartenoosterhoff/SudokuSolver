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
        }

        public void SetCell(int cellId, int value)
        {
            sudoku.Cells[cellId].SetValue(value);

            //OnCellValueSet(cellId, value);

            var cellIdsWithCandidatesToUnset = from g in FindGroupsForCell(sudoku, cellId)
                                               from c in g.CellIds
                                               where c != cellId
                                               select c;
            foreach (var candidateCellId in cellIdsWithCandidatesToUnset)
            {
                sudoku.RemoveCandidate(candidateCellId, value);
            }

            // Clear cache
            sudoku.GetCandidateBitSetCache().Clear();
        }

        public void RemoveCandidate(int cellId, int candidate)
        {
            if (!sudoku.Cells[cellId].Candidates[candidate])
            {
                return;
            }

            sudoku.Cells[cellId].RemoveCandidate(candidate);

            // Clear cache
            sudoku.GetCandidateBitSetCache().Clear();

            //OnCellCandidateRemoved(cellId, candidate);
        }
    }
}