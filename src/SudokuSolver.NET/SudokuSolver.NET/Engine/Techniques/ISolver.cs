using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal interface ISolver<in TIteration>
{
    SolveStep? Solve(Sudoku sudoku, TIteration iteration);
}