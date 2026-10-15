using System.Collections.Immutable;

namespace SudokuSolver.NET.Models;

internal sealed record Group(
    int Id,
    string Name,
    ImmutableArray<int> CellIds,
    BitSet OverlapGroups)
{
    public bool HasCell(int cellId)
    {
        return CellIds.Contains(cellId);
    }
}