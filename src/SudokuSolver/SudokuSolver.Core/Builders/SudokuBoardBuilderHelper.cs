using System;
using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Builders
{
    public static class SudokuBoardBuilderHelper
    {
        public static Cell[] CreateCells(int candidateCount, int cellCount)
        {
            var cells = Enumerable.Range(0, cellCount)
                .Select(i => new Cell(
                    i,
                    $"R{((i - (i % candidateCount)) / candidateCount) + 1}C{(i % candidateCount) + 1}",
                    Candidate.NotSet,
                    Enumerable.Range(0, candidateCount).Select(x => true).ToArray()
                ))
                .ToArray();
            return cells;
        }

        public static Group[] CreateDefaultGroups(Cell[] cells)
        {
            var gId = 0;
            var dim = (int)Math.Sqrt(cells.Length);
            var dimBlock = (int)Math.Sqrt(dim);
            var groups = new List<Group>();


            // Rows
            var rowGroups = from r in Enumerable.Range(0, dim)
                            select new Group(
                                gId++,
                                $"R{r + 1}",

                                Enumerable.Range(0, dim).Select(c => cells[(r * dim) + c].ID)
                            );
            groups.AddRange(rowGroups);

            // Columns
            var colGroups = from c in Enumerable.Range(0, dim)
                            select new Group(
                                gId++,
                                $"C{c + 1}",
                                Enumerable.Range(0, dim).Select(r => cells[(r * dim) + c].ID)
                            );
            groups.AddRange(colGroups);

            // Blocks
            var blockGroups = from r0 in Enumerable.Range(0, dim)
                              where r0 % dimBlock == 0
                              from c0 in Enumerable.Range(0, dim)
                              where c0 % dimBlock == 0
                              select new Group(
                                  gId++,
                                  $"B{r0 + 1}{c0 + 1}",
                                  from r in Enumerable.Range(r0, dimBlock)
                                  from c in Enumerable.Range(c0, dimBlock)
                                  select cells[(r * dim) + c].ID
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
                    g.OverlapGroups.Layer[gx.Id] = g.Cells.Any(c => gx.Cells.Contains(c));
                }
                g.OverlapGroups.Layer[g.Id] = false;
            }
        }
    }
}
