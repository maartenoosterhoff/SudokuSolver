using System;
using System.Linq;
using SudokuSolver.Core.Events;
using System.Collections.Generic;

namespace SudokuSolver.Core.Models
{
    public interface ISudokuBoardProxy
    {
        SudokuBoard SudokuBoard { get; }

        void SetCell(int cellId, int value);
        void RemoveCandidate(int cellId, int candidate);

        Group[] FindGroupsForCell(int cellId);
        bool GroupHasNumber(int groupId, int value);

        BitSet CandidateAsBitSet(int candidate);
        BitSet GroupAsBitSet(int groupId);
        void SetCandidateLayerWithBase(int candidate, bool value, BitSet baseLayer);
        int[] YieldCellIds(BitSet a);
        string YieldCellsDescription(BitSet a);
        int[] BitSetToCellIdArray(BitSet cellBitSet);

        event EventHandler<CellCandidateRemovedEventArgs> CellCandidateRemoved;
        event EventHandler<CellValueSetEventArgs> CellValueSet;
    }

    public class SudokuBoardProxy : ISudokuBoardProxy
    {
        public SudokuBoardProxy(SudokuBoard sudokuBoard)
        {
            if (sudokuBoard == null)
                throw new ArgumentNullException(nameof(sudokuBoard));

            SudokuBoard = sudokuBoard;
        }

        public SudokuBoard SudokuBoard { get; }

        public event EventHandler<CellCandidateRemovedEventArgs> CellCandidateRemoved;
        public event EventHandler<CellValueSetEventArgs> CellValueSet;

        private void OnCellCandidateRemoved(int cellId, int candidate)
        {
            CellCandidateRemoved?.Invoke(this, new CellCandidateRemovedEventArgs(cellId, candidate));
        }

        private void OnCellValueSet(int cellId, int value)
        {
            CellValueSet?.Invoke(this, new CellValueSetEventArgs(cellId, value));
        }

        public void SetCell(int cellId, int value)
        {
            SudokuBoard.Cells[cellId].SetValue(value);

            OnCellValueSet(cellId, value);

            var cellIdsWithCandidatesToUnset = from g in FindGroupsForCell(cellId)
                                               from c in g.CellIds
                                               where c != cellId
                                               select c;
            foreach (var candidateCellId in cellIdsWithCandidatesToUnset)
            {
                RemoveCandidate(candidateCellId, value);
            }

            // Clear cache
            _candidateAsBitSetCache.Clear();
        }

        public void RemoveCandidate(int cellId, int candidate)
        {
            if (SudokuBoard.Cells[cellId].Candidates[candidate])
            {
                SudokuBoard.Cells[cellId].Candidates[candidate] = false;

                // Clear cache
                _candidateAsBitSetCache.Clear();

                OnCellCandidateRemoved(cellId, candidate);
            }
        }

        public Group[] FindGroupsForCell(int cellId)
        {
            return SudokuBoard.Groups.Where(g => g.HasCell(cellId)).ToArray();
        }

        public bool GroupHasNumber(int groupId, int value)
        {
            return SudokuBoard.Groups[groupId].CellIds.Any(c => SudokuBoard.Cells[c].Value == value);
        }

        private readonly Dictionary<int, BitSet> _candidateAsBitSetCache = new Dictionary<int, BitSet>();

        public BitSet CandidateAsBitSet(int candidate)
        {
            if (!_candidateAsBitSetCache.ContainsKey(candidate))
            {
                var a = new BitSet(SudokuBoard.CellCount, false);
                for (int i = 0; i < SudokuBoard.Cells.Length; i++)
                {
                    a[i] = SudokuBoard.Cells[i].Candidates[candidate];
                }
                //return A;
                _candidateAsBitSetCache.Add(candidate, a);
                return a;
            }

            return _candidateAsBitSetCache[candidate];
        }

        private readonly Dictionary<int, BitSet> _groupAsBitSetCache = new Dictionary<int, BitSet>();

        public BitSet GroupAsBitSet(int groupId)
        {
            if (!_groupAsBitSetCache.ContainsKey(groupId))
            {
                var a = new BitSet(SudokuBoard.CellCount, false);
                foreach (var c in SudokuBoard.Groups[groupId].CellIds)
                {
                    a[c] = true;
                }
                //return A;
                _groupAsBitSetCache.Add(groupId, a);
                return a;
            }

            return _groupAsBitSetCache[groupId];
        }

        public int[] BitSetToCellIdArray(BitSet cellBitSet)
        {
#if DEBUG
            if (cellBitSet.Size != SudokuBoard.CellCount)
                throw new InvalidOperationException("Bit-set size is not equal to the solution dimension!");
#endif
            var cellIds = new List<int>();
            foreach (var c in SudokuBoard.Cells)
            {
                if (cellBitSet[c.Id])
                {
                    cellIds.Add(c.Id);
                }
            }
            return cellIds.ToArray();
        }

        public void SetCandidateLayerWithBase(int candidate, bool value, BitSet baseLayer)
        {
#if DEBUG
            if (baseLayer.Size != SudokuBoard.CellCount)
                throw new InvalidOperationException("Baselayer dimension is not equal to the solution dimension!");
#endif

            foreach (var c in SudokuBoard.Cells)
            {
                if (baseLayer[c.Id])
                {
                    c.Candidates[candidate] = value;
                }
            }
        }

        public string YieldCellsDescription(BitSet a)
        {
#if DEBUG
            if (a.Size != SudokuBoard.CellCount)
                throw new Exception("Bitlayer dimension is not equal to the solution dimension!");
#endif

            var cellNames = from cellId in YieldCellIds(a)
                            select SudokuBoard.Cells[cellId].Name;

            return string.Join(",", cellNames);
        }

        public int[] YieldCellIds(BitSet a)
        {
#if DEBUG
            if (a.Size != SudokuBoard.CellCount)
                throw new Exception("Bitlayer dimension is not equal to the solution dimension!");
#endif

            var cellIds = from i in Enumerable.Range(0, a.Size)
                          where a[i]
                          select SudokuBoard.Cells[i].Id;

            return cellIds.ToArray();
        }
    }
}