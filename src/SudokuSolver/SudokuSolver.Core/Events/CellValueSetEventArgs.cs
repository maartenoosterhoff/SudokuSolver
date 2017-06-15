using System;

namespace SudokuSolver.Core.Events
{
    public class CellValueSetEventArgs : EventArgs
    {
        public CellValueSetEventArgs(int cellId, int value)
        {
            CellId = cellId;
            Value = value;
        }

        public int CellId { get; }
        public int Value { get; }
    }
}
