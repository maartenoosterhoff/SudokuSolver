using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal abstract class TechniqueBase<TIterator, TSolver> : ITechnique where TIterator : new() where TSolver : new()
{
    protected TIterator Iterator { get; } = new();
    protected TSolver Solver { get; } = new();

    public abstract SolveStep? Solve(Sudoku sudoku);
}
