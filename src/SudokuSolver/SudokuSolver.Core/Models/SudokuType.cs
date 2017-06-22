using System;

namespace SudokuSolver.Core.Models
{
    public enum SudokuType
    {
        Classic9by9,
        Classic9by9Plus4,
        Sudoku16by16,
        XSudoku
    }

    public class SudokuTypeAttribute : Attribute {
        public SudokuType SudokuType { get; set; }
    }
}
