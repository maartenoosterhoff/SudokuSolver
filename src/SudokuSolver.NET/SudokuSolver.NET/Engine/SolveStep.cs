namespace SudokuSolver.NET.Engine;

public sealed record SolveStep(SolveStepItem[] Items);

public sealed record SolveStepItem(
    int[] CellIds,
    int Value,
    SolveStepItemType SolveStepType,
    string TechniqueName,
    string Explanation);

public enum SolveStepItemType
{
    CandidateRemoval,
    CandidateConfirmation
}