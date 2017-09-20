using System;

namespace SudokuSolver.Core.Models
{
    public enum SudokuType
    {
        Classic9By9,
        Classic9By9Plus4,
        Sudoku16By16,
        XSudoku
    }

    public class SudokuTypeAttribute : Attribute {
        public SudokuType SudokuType { get; set; }
    }
}
