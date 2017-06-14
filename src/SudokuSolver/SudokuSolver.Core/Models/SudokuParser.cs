using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SudokuSolver.Core.Extensions;

namespace SudokuSolver.Core.Models
{
    public class SudokuBoard
    {
        public SudokuType SudokuType { get; }
        public int CellCount { get; }
        public int CandidateCount { get; }
        public Cell[] Cells { get; }
        public Group[] Groups { get; set; }

        public SudokuBoard(SudokuType sudokuType, int cellCount, int candidateCount, Cell[] cells)
        {
            SudokuType = sudokuType;
            CellCount = cellCount;
            CandidateCount = candidateCount;
            Cells = cells;
        }
    }

    public interface ISudokuParser
    {
        SudokuBoard Parse(SudokuType sudokuType, string sudoku);
    }
    public class SudokuParser : ISudokuParser
    {
        private static readonly IReadOnlyDictionary<SudokuType, int> SudokuTypeToCandidateCountMapper = new ReadOnlyDictionary<SudokuType, int>(
            new Dictionary<SudokuType, int>
            {
                { SudokuType.Classic9by9, 9 },
                { SudokuType.Classic9by9Plus4, 9 },
                { SudokuType.XSudoku, 9 },
                { SudokuType.Sudoku16by16, 16 }
            }
        );

        public SudokuBoard Parse(SudokuType sudokuType, string sudoku)
        {
            if (!Enum.IsDefined(typeof(SudokuType), sudokuType))
                throw new ArgumentException("Parameter sudokuType should have a valid enum value.", nameof(sudokuType));

            if (!SudokuTypeToCandidateCountMapper.ContainsKey(sudokuType))
                throw new ArgumentException("Unknown Sudokutype!", nameof(sudokuType));

            var candidateCount = SudokuTypeToCandidateCountMapper[sudokuType];

            var cellCount = candidateCount * candidateCount;
            var cells = Enumerable.Range(0, cellCount)
                .Select(i => new Cell(i, "R" + (((i - (i % candidateCount)) / candidateCount) + 1).ToString() + "C" + ((i % candidateCount) + 1).ToString()))
                .ToArray();

            var sudokuBoard = new SudokuBoard(
                sudokuType,
                cellCount,
                candidateCount,
                cells
            );

            sudokuBoard.Groups = CreateGroups(sudokuBoard);

            PopulateGroupOverlapData(sudokuBoard);

            return sudokuBoard;
        }

        private static Group[] CreateGroups(SudokuBoard sudokuBoard)
        {
            Group g;
            int dim = -1, dimBlock = -1;
            int gId = 0;

            var groups = new List<Group>();

            switch (sudokuBoard.SudokuType)
            {
                case SudokuType.Classic9by9:
                case SudokuType.Classic9by9Plus4:
                case SudokuType.XSudoku:
                    dim = 9;
                    dimBlock = 3;

                    break;
                case SudokuType.Sudoku16by16:
                    dim = 16;
                    dimBlock = 4;
                    break;

                default:
                    break;
            }

            if (sudokuBoard.SudokuType.In(SudokuType.Classic9by9, SudokuType.Classic9by9Plus4, SudokuType.Sudoku16by16, SudokuType.XSudoku))
            {
                // Rows
                var rowGroups = from r in Enumerable.Range(0, dim)
                                select new Group(
                                    gId++,
                                    "R" + (r + 1).ToString(),
                                    Enumerable.Range(0, dim).Select(c => sudokuBoard.Cells[(r * dim) + c].ID)
                                );
                groups.AddRange(rowGroups);

                // Columns
                var colGroups = from c in Enumerable.Range(0, dim)
                                select new Group(
                                    gId++,
                                    "C" + (c + 1).ToString(),
                                    Enumerable.Range(0, dim).Select(r => sudokuBoard.Cells[(r * dim) + c].ID)
                                );
                groups.AddRange(colGroups);

                // Blocks
                var blockGroups = from r0 in Enumerable.Range(0, dim)
                                  where r0 % dimBlock == 0
                                  from c0 in Enumerable.Range(0, dim)
                                  where c0 % dimBlock == 0
                                  select new Group(
                                      gId++,
                                      "B" + (r0 + 1).ToString() + (c0 + 1).ToString(),
                                      from r in Enumerable.Range(r0, dimBlock)
                                      from c in Enumerable.Range(c0, dimBlock)
                                      select sudokuBoard.Cells[(r * dim) + c].ID
                                 );
                groups.AddRange(blockGroups);
            }
            if (sudokuBoard.SudokuType == SudokuType.Classic9by9Plus4)
            {
                groups.Add(
                    new Group(
                        gId++,
                        "B22",
                        from r in Enumerable.Range(1, 3)
                        from c in Enumerable.Range(1, 3)
                        select sudokuBoard.Cells[(r * dim) + c].ID
                    )
                );

                groups.Add(
                    new Group(
                        gId++,
                        "B62",
                        from r in Enumerable.Range(5, 3)
                        from c in Enumerable.Range(1, 3)
                        select sudokuBoard.Cells[(r * dim) + c].ID
                    )
                );

                groups.Add(
                    new Group(
                        gId++,
                        "B26",
                        from r in Enumerable.Range(1, 3)
                        from c in Enumerable.Range(5, 3)
                        select sudokuBoard.Cells[(r * dim) + c].ID
                    )
                );

                groups.Add(
                    new Group(
                        gId++,
                        "B66",
                        from r in Enumerable.Range(5, 3)
                        from c in Enumerable.Range(5, 3)
                        select sudokuBoard.Cells[(r * dim) + c].ID
                    )
                );
            }

            if (sudokuBoard.SudokuType == SudokuType.XSudoku)
            {
                groups.Add(
                    new Group(
                        gId++,
                        "D1",
                        from i in Enumerable.Range(0, dim)
                        select sudokuBoard.Cells[(i * dim) + i].ID
                    )
                );

                groups.Add(
                    new Group(
                        gId++,
                        "D2",
                        from i in Enumerable.Range(0, dim)
                        select sudokuBoard.Cells[(i * dim) + dim - i - 1].ID
                    )
                );
            }

            return groups.ToArray();
        }

        public static void PopulateGroupOverlapData(SudokuBoard sudokuBoard)
        {
            var max = sudokuBoard.Groups.Length;
            foreach (var g in sudokuBoard.Groups)
            {
                g.OverlapGroups = new BitLayer(max, false);
                foreach (var gx in sudokuBoard.Groups)
                {
                    g.OverlapGroups.Layer[gx.Id] = g.HasOverlapWithGroup(gx);
                }
                g.OverlapGroups.Layer[g.Id] = false;
            }
        }
    }
}