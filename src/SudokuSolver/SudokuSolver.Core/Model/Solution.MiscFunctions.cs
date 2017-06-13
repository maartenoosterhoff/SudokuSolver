using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core
{
    partial class Solution
    {
        ///// <summary>
        ///// Finds a group with the given name.
        ///// </summary>
        ///// <param name="blockName">The name to look for</param>
        ///// <returns>The group if found, else null</returns>
        //public Group FindGroup(string blockName)
        //{
        //    return _groupList
        //        .Where(g => string.Compare(g.Name, blockName) == 0)
        //        .FirstOrDefault();
        //    //foreach (Group g in _groupList) {
        //    //    if (string.Compare(g.Name, blockName) == 0) {
        //    //        return g;
        //    //    }
        //    //}
        //    //return null;
        //}

        /// <summary>
        /// Finds groups to which the given cell belongs to.
        /// </summary>
        /// <param name="cellId">The cell</param>
        /// <returns>A list of groups</returns>
        public List<Group> FindGroupsForCell(int cellId)
        {
            return _groupList
                .Where(g => g.HasCell(cellId))
                .ToList();

            //List<Group> l = new List<Group>();
            //foreach (Group g in _groupList) {
            //    if (g.HasCell(cellId)) l.Add(g);
            //}
            //return l;
        }

        /// <summary>
        /// Set a certain cell to a certain value. Remove all candidates.
        /// </summary>
        /// <param name="cellId">The cell to set</param>
        /// <param name="value">The value to set the cell to</param>
        public void SetCell(int cellId, int value)
        {
            _cellList[cellId].Value = value;

            var cellsWithCandidatesToUnset = from g in FindGroupsForCell(cellId)
                                             from c in g.Cells
                                             where c != cellId
                                             select _cellList[c];
            foreach (var cell in cellsWithCandidatesToUnset)
            {
                cell.Candidates[value] = false;
            }

            //foreach (var g in FindGroupsForCell(cellId))
            //{
            //    foreach (int c in g.Cells)
            //    {
            //        if (c != cellId)
            //        {
            //            _cellList[c].Candidates[value] = false;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Checks if a group has a cell which has a certain value.
        /// For example, is there a value 5 in row 2.
        /// </summary>
        /// <param name="groupId">The group to look in</param>
        /// <param name="value">The value to look for</param>
        /// <returns>True if the given value is found, else False</returns>
        public bool GroupHasNumber(int groupId, int value)
        {
            return _groupList[groupId].Cells.Any(c => _cellList[c].Value == value);


            //var result = false;
            //var cells = _groupList[groupId].Cells;
            //for (var c = 0; c < cells.Count && !result; c++)
            //    result = _cellList[cells[c]].Value == value;

            //return result;
        }

        /// <summary>
        /// Converts a group to a SolutionBitLayer, with every cell in the group as True, the rest as False.
        /// Similar to Group.AsBitLayer, but this method always returns a BitLayer equal in dimensionsize to
        /// the solution.
        /// </summary>
        /// <param name="groupId">The group to convert</param>
        /// <returns>The resulting SolutionBitLayer</returns>
        public BitLayer GroupAsBitLayer(int groupId)
        {
            var A = new BitLayer(Dimension, false);
            foreach (var c in _groupList[groupId].Cells)
            {
                A.Layer[c] = true;
            }
            return A;
        }

        /// <summary>
        /// Converts a certain candidate of all cells to a BitLayer.
        /// </summary>
        /// <param name="candidate">The candidate to look up</param>
        /// <returns>The BitLayer representing the given candidate</returns>
        public BitLayer CandidateAsBitLayer(int candidate)
        {
            var A = new BitLayer(Dimension, false);
            foreach (var c in _cellList)
            {
                A.Layer[c.ID] = c.Candidates[candidate];
            }
            return A;
        }

        /// <summary>
        /// Converts the having of a value or not of all cells to a BitLayer.
        /// </summary>
        /// <returns>The resulting BitLayer</returns>
        public BitLayer ValuesAsBitLayer()
        {
            var A = new BitLayer(Dimension, false);
            foreach (var c in _cellList)
            {
                A.Layer[c.ID] = (c.Value != Candidate.NotSet);
            }
            return A;
        }

        /// <summary>
        /// Sets the specified candidate of all cells in the baseLayer to the giving value.
        /// </summary>
        /// <param name="candidate">The candidate to set</param>
        /// <param name="value">The value to set the candidate to</param>
        /// <param name="baseLayer">The baselayer with the cells to alter</param>
        public void SetCandidateLayerWithBase(int candidate, bool value, BitLayer baseLayer)
        {
            if (!baseLayer.EqualToSolutionDimension)
                throw new Exception("Baselayer dimension is not equal to the solution dimension!");

            foreach (var c in _cellList)
            {
                if (baseLayer.Layer[c.ID])
                {
                    c.Candidates[candidate] = value;
                }
            }
        }

        /// <summary>
        /// Sets the specified cells in the baselayer to the value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="baseLayer">The baselayer</param>
        public void SetCellWithBase(int value, BitLayer baseLayer)
        {
            if (!baseLayer.EqualToSolutionDimension)
                throw new Exception("Baselayer dimension is not equal to the solution dimension!");

            foreach (var c in _cellList)
            {
                if (baseLayer.Layer[c.ID])
                {
                    SetCell(c.ID, value);
                }
            }
        }


        /// <summary>
        /// Create a string with all the cells in the BitLayer.
        /// </summary>
        /// <param name="A">The BitLayer to textify</param>
        /// <returns>The string with the cells</returns>
        public string YieldCells(BitLayer A)
        {
            if (!A.EqualToSolutionDimension)
                throw new Exception("Bitlayer dimension is not equal to the solution dimension!");

            string cells = string.Empty;
            for (var i = 0; i < A.Dimension; i++)
            {
                if (A.Layer[i])
                {
                    if (!string.IsNullOrEmpty(cells)) { cells += ","; }
                    cells += _cellList[i].Name;
                }
            }

            return cells;
        }

        /// <summary>
        /// Find all the groups that share cells with this group.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <returns>A list of group-id's that share cells with this group</returns>
        [Obsolete("Use the OverlapGroups property of the Group Class instead.", true)]
        public List<int> FindGroupsWithOverlap(int groupId)
        {
            var l = new List<int>();
            for (var groupIdX = 0; groupIdX < _groupList.Count; groupIdX++)
                if (groupId != groupIdX)
                    if (_groupList[groupId].HasOverlapWithGroup(_groupList[groupIdX]))
                        l.Add(groupIdX);
            return l;
        }

        /// <summary>
        /// Find all groups which don't have a cell which has a certain value.
        /// </summary>
        /// <param name="value">A certain value</param>
        /// <returns>A list of group-id's</returns>
        public bool[] FindGroupsWithoutValue(int value)
        {
            var groupsWithoutValue = from groupId in Enumerable.Range(0, _groupList.Count)
                                     select !GroupHasNumber(groupId, value);

            return groupsWithoutValue.ToArray();

            //var l = new bool[_groupList.Count];
            //for (var groupId = 0; groupId < _groupList.Count; groupId++)
            //    l[groupId] = !GroupHasNumber(groupId, value);
            //return l;
        }

        /// <summary>
        /// Populate the internal data of the groups, so it can be easily checked if one group shares cells with another group.
        /// </summary>
        public void PopulateGroupOverlapData()
        {
            var max = _groupList.Count;
            foreach (var g in _groupList)
            {
                g.OverlapGroups = new BitLayer(max, false);
                foreach (var gx in _groupList)
                {
                    g.OverlapGroups.Layer[gx.Id] = g.HasOverlapWithGroup(gx);
                }
                g.OverlapGroups.Layer[g.Id] = false;
            }
        }

        /// <summary>
        /// Counts the amount of possible candidates for a certain value in a group.
        /// </summary>
        /// <param name="groupId">The group to look in</param>
        /// <param name="value">The candidate to count</param>
        /// <returns></returns>
        public int GroupCandidateCount(int groupId, int value)
        {
            return _groupList[groupId].Cells.Count(c => _cellList[c].Candidates[value]);

            //var count = 0;
            //for (var c = 0; c < cells.Count; c++)
            //    if (_cellList[cells[c]].Candidates[value])
            //        count++;

            //return count;
        }
    }
}
