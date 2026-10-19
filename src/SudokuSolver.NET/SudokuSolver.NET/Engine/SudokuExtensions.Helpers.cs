using SudokuSolver.NET.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace SudokuSolver.NET.Engine;

internal static partial class SudokuExtensions
{
    extension(Sudoku sudoku)
    {
        public ImmutableArray<Group> FindGroupsForCell(int cellId)
        {
            return [.. sudoku.Groups.Where(g => g.Cells.Any(x => x.Id == cellId))];
        }

        public bool GroupHasNumber(int groupId, int value)
        {
            return sudoku.Groups[groupId].Cells.Any(c => c.Value == value);
        }

        public BitSet GroupAsBitSet(int groupId)
        {
            return sudoku.GetGroupAsBitSetCache().GetOrAdd(
                groupId,
                _ =>
                {
                    var a = new BitSet(sudoku.CellCount, false);
                    foreach (var cell in sudoku.Groups[groupId].Cells)
                    {
                        a[cell.Id] = true;
                    }

                    return a;
                });
        }

        public BitSet CandidateAsBitSet(int candidate)
        {
            return sudoku.GetCandidateBitSetCache().GetOrAdd(
                candidate,
                _ =>
                {
                    var a = new BitSet(sudoku.CellCount, false);
                    for (var i = 0; i < sudoku.Cells.Length; i++)
                    {
                        a[i] = sudoku.Cells[i].Candidates[candidate];
                    }

                    return a;
                });
        }

        public int[] BitSetToCellIdArray(BitSet cellBitSet)
        {
            if (cellBitSet.Size != sudoku.CellCount)
            {
                throw new InvalidOperationException("Bit-set size is not equal to the solution dimension!");
            }

            return [.. sudoku.Cells.Where(c => cellBitSet[c.Id]).Select(c => c.Id)];
        }

        public string YieldCellsDescription(BitSet a)
        {
            if (a.Size != sudoku.CellCount)
            {
                throw new Exception("BitLayer dimension is not equal to the solution dimension!");
            }

            var cellNames = from cellId in sudoku.YieldCellIds(a) select sudoku.Cells[cellId].Name;

            return string.Join(",", cellNames);
        }

        public int[] YieldCellIds(BitSet a)
        {
            if (a.Size != sudoku.CellCount)
            {
                throw new Exception("BitLayer dimension is not equal to the solution dimension!");
            }

            var cellIds = from i in Enumerable.Range(0, a.Size)
                          where a[i]
                          select sudoku.Cells[i].Id;

            return cellIds.ToArray();
        }

        private ConcurrentDictionary<int, BitSet> GetCandidateBitSetCache() => CandidateBitSetCache.GetOrAdd(sudoku, _ => new());
        private ConcurrentDictionary<int, BitSet> GetGroupAsBitSetCache() => GroupBitSetCache.GetOrAdd(sudoku, _ => new());
    }

    private static readonly ConcurrentDictionary<Sudoku, ConcurrentDictionary<int, BitSet>> CandidateBitSetCache = new();
    private static readonly ConcurrentDictionary<Sudoku, ConcurrentDictionary<int, BitSet>> GroupBitSetCache = new();
}