using SudokuSolver.NET.Models;
using SudokuSolver.NET.Parsers;
using System.Collections.Immutable;

namespace SudokuSolver.NET.Builders;

internal class SudokuBuilder
{
    public static SudokuBuilder SomeSudoku => new SudokuBuilder();

    private SudokuBuilder()
    {
    }

    private SudokuType _sudokuType = SudokuType.Classic9By9;
    private string? _contents = null;

    public SudokuBuilder WithType(SudokuType sudokuType)
    {
        _sudokuType = sudokuType;
        return this;
    }

    public SudokuBuilder WithContents(string contents)
    {
        _contents = contents;
        return this;
    }

    public Sudoku Build()
    {
        var sudoku = _sudokuType switch
        {
            SudokuType.Classic9By9 => Create(_sudokuType, 81, 9),
            _ => throw new NotSupportedException()
        };

        if (_contents is not null)
        {
            sudoku.Parse(_contents);
        }

        return sudoku;
    }

    private static Sudoku Create(SudokuType sudokuType, int cellCount, int candidateCount)
    {
        ImmutableArray<Cell> cells = [
            ..
            Enumerable.Range(0, cellCount)
                .Select(i => new Cell(
                    i,
                    $"R{(i - i % candidateCount) / candidateCount + 1}C{i % candidateCount + 1}",
                    Candidate.NotSet,
                    [.. Enumerable.Range(0, candidateCount).Select(x => true)]))
                .ToArray()
        ];

        var partialGroups = CreateDefaultPartialGroups(cells);
        var groups = CreateGroupsWithOverlapData(partialGroups);

        return new(sudokuType, cellCount, candidateCount, cells, groups);
    }

    private static ImmutableArray<PartialGroup> CreateDefaultPartialGroups(ImmutableArray<Cell> cells)
    {
        var gId = 0;
        var dim = (int)Math.Sqrt(cells.Length);
        var dimBlock = (int)Math.Sqrt(dim);
        var groups = new List<PartialGroup>();


        // Rows
        var rowGroups = from r in Enumerable.Range(0, dim)
                        select new PartialGroup(
                            gId++,
                            $"R{r + 1}",

                            [.. Enumerable.Range(0, dim).Select(c => cells[(r * dim) + c].Id)]
                        );
        groups.AddRange(rowGroups);

        // Columns
        var colGroups = from c in Enumerable.Range(0, dim)
                        select new PartialGroup(
                            gId++,
                            $"C{c + 1}",
                            [.. Enumerable.Range(0, dim).Select(r => cells[(r * dim) + c].Id)]
                        );
        groups.AddRange(colGroups);

        // Blocks
        var blockGroups = from r0 in Enumerable.Range(0, dim)
                          where r0 % dimBlock == 0
                          from c0 in Enumerable.Range(0, dim)
                          where c0 % dimBlock == 0
                          select new PartialGroup(
                              gId++,
                              $"B{r0 + 1}{c0 + 1}",
                              [ ..
                                  from r in Enumerable.Range(r0, dimBlock)
                                  from c in Enumerable.Range(c0, dimBlock)
                                  select cells[(r * dim) + c].Id
                              ]
                         );
        groups.AddRange(blockGroups);

        //if (sudokuBoard.SudokuType == SudokuType.Classic9by9Plus4)
        //{
        //    groups.Add(
        //        new Group(
        //            gId++,
        //            "B22",
        //            from r in Enumerable.Range(1, 3)
        //            from c in Enumerable.Range(1, 3)
        //            select sudokuBoard.Cells[(r * dim) + c].ID
        //        )
        //    );

        //    groups.Add(
        //        new Group(
        //            gId++,
        //            "B62",
        //            from r in Enumerable.Range(5, 3)
        //            from c in Enumerable.Range(1, 3)
        //            select sudokuBoard.Cells[(r * dim) + c].ID
        //        )
        //    );

        //    groups.Add(
        //        new Group(
        //            gId++,
        //            "B26",
        //            from r in Enumerable.Range(1, 3)
        //            from c in Enumerable.Range(5, 3)
        //            select sudokuBoard.Cells[(r * dim) + c].ID
        //        )
        //    );

        //    groups.Add(
        //        new Group(
        //            gId++,
        //            "B66",
        //            from r in Enumerable.Range(5, 3)
        //            from c in Enumerable.Range(5, 3)
        //            select sudokuBoard.Cells[(r * dim) + c].ID
        //        )
        //    );
        //}

        //if (sudokuBoard.SudokuType == SudokuType.XSudoku)
        //{
        //    groups.Add(
        //        new Group(
        //            gId++,
        //            "D1",
        //            from i in Enumerable.Range(0, dim)
        //            select sudokuBoard.Cells[(i * dim) + i].ID
        //        )
        //    );

        //    groups.Add(
        //        new Group(
        //            gId++,
        //            "D2",
        //            from i in Enumerable.Range(0, dim)
        //            select sudokuBoard.Cells[(i * dim) + dim - i - 1].ID
        //        )
        //    );
        //}

        return [.. groups];
    }

    private static ImmutableArray<Group> CreateGroupsWithOverlapData(ImmutableArray<PartialGroup> partialGroups)
    {
        var groups = new List<Group>();
        var max = partialGroups.Length;
        foreach (var g in partialGroups)
        {
            var overlapGroups = new BitSet(max, false);

            foreach (var gx in partialGroups)
            {
                overlapGroups[gx.Id] = g.CellIds.Any(c => gx.CellIds.Contains(c));
            }
            overlapGroups[g.Id] = false;

            groups.Add(new Group(g.Id, g.Name, g.CellIds, overlapGroups));
        }

        return [.. groups];
    }

    private sealed record PartialGroup(int Id, string Name, ImmutableArray<int> CellIds);
}