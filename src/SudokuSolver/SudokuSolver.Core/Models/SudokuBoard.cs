using System;

namespace SudokuSolver.Core.Models
{
    public class SudokuBoard
    {
        public SudokuType SudokuType { get; }
        public int CellCount { get; }
        public int CandidateCount { get; }
        public Cell[] Cells { get; }
        public Group[] Groups { get; set; }
        public SudokuState State { get; set; }

        public SudokuBoard(SudokuType sudokuType, int cellCount, int candidateCount, Cell[] cells)
        {
            if (!Enum.IsDefined(typeof(SudokuType), sudokuType))
                throw new ArgumentException("Argument is not a valid value.", nameof(sudokuType));
            if (cellCount <= 0)
                throw new ArgumentException("Argument should be a positive value.", nameof(cellCount));
            if (candidateCount <= 0)
                throw new ArgumentException("Argument should be a positive value.", nameof(cellCount));
            if (cells == null)
                throw new ArgumentNullException(nameof(cells));
            if (cells.Length == 0)
                throw new ArgumentException("Argument should have a content.", nameof(cells));

            SudokuType = sudokuType;
            CellCount = cellCount;
            CandidateCount = candidateCount;
            Cells = cells;

            State = SudokuState.Reset;
        }
    }
}