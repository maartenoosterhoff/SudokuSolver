using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal interface ITechnique
{
    SolveStep? Solve(Sudoku sudoku);
}

