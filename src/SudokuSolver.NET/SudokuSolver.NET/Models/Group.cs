using System.Collections.Immutable;

namespace SudokuSolver.NET.Models;

internal sealed record Group(
    int Id,
    string Name,
    ImmutableArray<Cell> Cells,
    BitSet OverlapGroups);