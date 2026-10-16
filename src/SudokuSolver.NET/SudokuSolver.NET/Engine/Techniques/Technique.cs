namespace SudokuSolver.NET.Engine.Techniques;

[Flags]
internal enum Technique
{
    NakedSingle = 1,
    NakedMultiple = 2,
    HiddenSingle = 4,
    HiddenMultiple = 8,

    LockedCandidate = 16,
    AlignedPairExclusion = 32,

    All =
        NakedSingle |
        NakedMultiple |
        HiddenSingle |
        HiddenMultiple |
        LockedCandidate |
        AlignedPairExclusion
}