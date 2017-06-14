using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to represent a group of cells in a sudoku, which for example can be a row, column or block
    /// in a classic sudoku.
    /// </summary>
    public class Group
    {
        #region Properties

        //private int _id;
        private List<int> _cells;
        //private string _name;
        private BitLayer _overlapGroups;

        /// <summary>
        /// The ID of the group. Read-only.
        /// </summary>
        public int Id { get; }
        //{
        //    get { return _id; }
        //}

        /// <summary>
        /// The name of the group. Read-only.
        /// </summary>
        public string Name { get; }
        //{
        //    get { return _name; }
        //    //set { _name = value; }
        //}

        /// <summary>
        /// The cells included in this group.
        /// </summary>
        public List<int> Cells
        {
            get { return _cells; }
            set { _cells = value; }
        }

        /// <summary>
        /// The groups which share cells with this group.
        /// </summary>
        public BitLayer OverlapGroups
        {
            get { return _overlapGroups; }
            set { _overlapGroups = value; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with the name of the group as a parameter.
        /// </summary>
        /// <param name="name">The name of the group</param>
        public Group(int id, string name)
        {
            Id = id;
            Name = name;
            _cells = new List<int>();
        }

        public Group(int id, string name, IEnumerable<int> cellIds)
        {
            Id = id;
            Name = name;
            _cells = cellIds.ToList();
        }

        #endregion

        /// <summary>
        /// Adds a cell to a group.
        /// </summary>
        /// <param name="cellId">The cell to add</param>
        public void AddCell(int cellId)
        {
            _cells.Add(cellId);
        }

        /// <summary>
        /// Check if a cell is included in a group.
        /// </summary>
        /// <param name="cellId">The cell to check</param>
        /// <returns>True if the cell is included in the group</returns>
        public bool HasCell(int cellId)
        {
            return _cells.Contains(cellId);
        }

        /// <summary>
        /// Converts the group to a BitLayer, with every cell in the group as True, and the rest as False.
        /// </summary>
        /// <param name="dimension">The dimension of the returned BitLayer</param>
        /// <returns>The BitLayer of the group</returns>
        public BitLayer AsBitLayer(int dimension)
        {
            BitLayer result = new BitLayer(dimension, false);

            foreach (int cellId in _cells)
                if (cellId < dimension)
                    result.Layer[cellId] = true;

            return result;
        }

        /// <summary>
        /// Checks if this group has overlap with another specified group. That means, if the two groups share
        /// some cells.
        /// This function could be like this: !((this & A).IsEmpty())
        /// But I think this implementation is quicker.
        /// </summary>
        /// <param name="A">Group to check overlap with</param>
        /// <returns>True if there is overlap, False is not</returns>
        public bool HasOverlapWithGroup(Group A)
        {
            return Cells.Any(c => A.Cells.Contains(c));
        }
    }
}

