using System.Collections.Immutable;

namespace SudokuSolver.NET.Models;

internal class Sudoku(SudokuType sudokuType, int cellCount, int candidateCount, ImmutableArray<Cell> cells, ImmutableArray<Group> groups)
{
    public SudokuType SudokuType { get; } = sudokuType;
    public int CellCount { get; } = cellCount;
    public int CandidateCount { get; } = candidateCount;
    public ImmutableArray<Cell> Cells { get; } = cells;
    public ImmutableArray<Group> Groups { get; } = groups;
    public SudokuState State { get; set; } = SudokuState.Reset;
}