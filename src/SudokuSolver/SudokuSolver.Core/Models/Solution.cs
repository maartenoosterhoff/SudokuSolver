using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core.Models
{
    enum SudokuState
    {
        RESET, SOLVED, UNSOLVED, BROKEN, UNSOLVABLE
    }
    
    /// <summary>
    /// Class used to represent a single solution of a sudoku.
    /// </summary>
    partial class Solution
    {
        #region Static Properties
        
        /// <summary>
        /// Specifies the dimension of the array representing the sudoku.
        /// </summary>
        //static private int _dimension;
        public static int Dimension { get; private set; }
        //{
        //    get { return _dimension; }
        //}
        #endregion

        #region Properties

        /// <summary>
        /// Specifies the state of the sudoku. Like UNSOLVED, SOLVED, etc
        /// </summary>
        private SudokuState _sudokuState;

        public SudokuState SudokuState
        {
            get { return _sudokuState; }
            set { _sudokuState = value; }
        }

        /// <summary>
        /// Specifies the type of sudoku.
        /// </summary>
        private SudokuType _sudokuType;

        /// <summary>
        /// The internal array of cells of the sudoku.
        /// </summary>
        private Cell[] _cellList;

        /// <summary>
        /// The internal list of groups defining the different groups of the sudoku. Can be rows, columns, etc.
        /// </summary>
        private List<Group> _groupList;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with the type of the sudoku as parameters.
        /// </summary>
        /// <param name="sudokuType">The type of the sudoku</param>
        public Solution(SudokuType sudokuType)
        {
            _sudokuType = sudokuType;
            _groupList = new List<Group>();

            CreateSudoku();
        }
        #endregion

        /// <summary>
        /// Resets the sudoku.
        /// </summary>
        public void Reset()
        {
            CreateSudoku();
        }

        /// <summary>
        /// Checks the state of the sudoku and alters the _sudokuState variable.
        /// </summary>
        public void CheckState()
        {
            // Check in each group for doubles and empties
            bool emptyCell = false;
            bool doubleValue = false;
            int[] filledValue;

            for (int gId = 0; gId < this._groupList.Count; gId++) {
                filledValue = new int[Candidate.PossibleCandidateCount];
                foreach (int cId in this._groupList[gId].Cells)
                    if (this._cellList[cId].Value == Candidate.NotSet)
                        emptyCell = true;
                    else
                        filledValue[this._cellList[cId].Value]++;
                foreach (int v in filledValue)
                    if (v > 1) doubleValue = true;
            }

            if (!emptyCell && !doubleValue)
                _sudokuState = SudokuState.SOLVED;
            else if (doubleValue)
                _sudokuState = SudokuState.BROKEN;
            else if (emptyCell)
                _sudokuState = SudokuState.UNSOLVED;
        }
    }
}
