using System;
using System.Linq;
using SudokuSolver.Core.Events;

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
        int[] YieldCellIds(BitSet A);
        string YieldCellsDescription(BitSet A);

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

        //public bool[] GetAllCandidates()
        //{
        //    return Enumerable.Range(0, SudokuBoard.CandidateCount).Select(x => true).ToArray();
        //}

        //public bool[] GetNoCandidates()
        //{
        //    return Enumerable.Range(0, SudokuBoard.CandidateCount).Select(x => false).ToArray();
        //}

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
                                               from c in g.Cells
                                               where c != cellId
                                               select c;
            foreach (var candidateCellId in cellIdsWithCandidatesToUnset)
            {
                RemoveCandidate(candidateCellId, value);
            }
        }

        public void RemoveCandidate(int cellId, int candidate)
        {
            SudokuBoard.Cells[cellId].Candidates[candidate] = false;

            OnCellCandidateRemoved(cellId, candidate);
        }

        public Group[] FindGroupsForCell(int cellId)
        {
            return SudokuBoard.Groups.Where(g => g.HasCell(cellId)).ToArray();
        }

        public bool GroupHasNumber(int groupId, int value)
        {
            return SudokuBoard.Groups[groupId].Cells.Any(c => SudokuBoard.Cells[c].Value == value);
        }

        public BitSet CandidateAsBitSet(int candidate)
        {
            var A = new BitSet(SudokuBoard.CellCount, false);
            for (int i = 0; i < SudokuBoard.Cells.Length; i++)
            {
                A[i] = SudokuBoard.Cells[i].Candidates[candidate];
            }
            //foreach (var cell in SudokuBoard.Cells)
            //{
            //    A[cell.ID] = cell.Candidates[candidate];
            //}
            return A;
        }

        public BitSet GroupAsBitSet(int groupId)
        {
            var A = new BitSet(SudokuBoard.CellCount, false);
            foreach (var c in SudokuBoard.Groups[groupId].Cells)
            {
                A[c] = true;
            }
            return A;
        }

        public void SetCandidateLayerWithBase(int candidate, bool value, BitSet baseLayer)
        {
            if (baseLayer.Size != SudokuBoard.CellCount)
                throw new InvalidOperationException("Baselayer dimension is not equal to the solution dimension!");

            foreach (var c in SudokuBoard.Cells)
            {
                if (baseLayer[c.ID])
                {
                    c.Candidates[candidate] = value;
                }
            }
        }

        public string YieldCellsDescription(BitSet A)
        {
            if (A.Size != SudokuBoard.CellCount)
                throw new Exception("Bitlayer dimension is not equal to the solution dimension!");

            var cellNames = from cellId in YieldCellIds(A)
                            select SudokuBoard.Cells[cellId].Name;

            return string.Join(",", cellNames);
        }

        public int[] YieldCellIds(BitSet A)
        {
            if (A.Size != SudokuBoard.CellCount)
                throw new Exception("Bitlayer dimension is not equal to the solution dimension!");

            var cellIds = from i in Enumerable.Range(0, A.Size)
                          where A[i]
                          select SudokuBoard.Cells[i].ID;

            return cellIds.ToArray();
        }
    }
}