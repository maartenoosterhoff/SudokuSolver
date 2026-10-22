using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal interface IIterator<out TIteration>
{
    IEnumerable<TIteration> Iterate(Sudoku sudoku);
}