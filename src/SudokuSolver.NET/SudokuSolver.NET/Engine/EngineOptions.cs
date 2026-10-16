using SudokuSolver.NET.Engine.Techniques;

namespace SudokuSolver.NET.Engine;

internal sealed class EngineOptions
{
    public TechniquesBuilder Techniques { get; } = TechniquesBuilder.All;

    public bool DisplayBetweenSteps { get; set; } = false;
}