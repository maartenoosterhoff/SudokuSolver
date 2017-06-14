using System;

namespace SudokuSolver.Core.Models
{
    class CellSorter: IComparable
    {
        #region Properties
        //private int _cellId;
        //private int _candidateCount;

        public int CellId { get; }
        //{
        //    get { return _cellId; }
        //    set { _cellId = value; }
        //}

        public int CandidateCount { get; }
        //{
        //    get { return _candidateCount; }
        //    set { _candidateCount = value; }
        //}
        #endregion

        public CellSorter(int cellId, int candidateCount)
        {
            CellId = cellId;
            CandidateCount = candidateCount;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null) {
                return 1;
            }
            else {
                CellSorter cell = (CellSorter)(obj);
                int compareToReturnValue = CandidateCount.CompareTo(cell.CandidateCount);
                if (compareToReturnValue == 0) {
                    compareToReturnValue = CellId.CompareTo(cell.CellId);
                }
                return compareToReturnValue;
            }
        }

        #endregion
    }
}
