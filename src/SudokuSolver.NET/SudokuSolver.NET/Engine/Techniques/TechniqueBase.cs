using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal abstract class TechniqueBase<TIterator, TSolver, TIteration> : ITechnique
    where TIterator : IIterator<TIteration>, new()
    where TSolver : ISolver<TIteration>, new()
{
    protected TIterator Iterator { get; } = new();
    protected TSolver Solver { get; } = new();

    public virtual SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Iterate(sudoku);
        var query =
            from item in items
            let solveStep = Solver.Solve(sudoku, item)
            where solveStep is not null
            select solveStep;

        return query.FirstOrDefault();
    }
}