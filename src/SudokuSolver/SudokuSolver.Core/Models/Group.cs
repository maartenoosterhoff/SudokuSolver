using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    public class Group
    {
        public int Id { get; }
        public string Name { get; }
        public List<int> Cells { get; set; }
        public BitLayer OverlapGroups { get; set; }

        public Group(int id, string name)
        {
            Id = id;
            Name = name;
            Cells = new List<int>();
        }

        public Group(int id, string name, IEnumerable<int> cellIds)
        {
            Id = id;
            Name = name;
            Cells = cellIds.ToList();
        }

        public void AddCell(int cellId)
        {
            Cells.Add(cellId);
        }

        public bool HasCell(int cellId)
        {
            return Cells.Contains(cellId);
        }

        public BitLayer AsBitLayer(int dimension)
        {
            BitLayer result = new BitLayer(dimension, false);

            foreach (int cellId in Cells)
                if (cellId < dimension)
                    result.Layer[cellId] = true;

            return result;
        }

        [Obsolete("Do not use", true)]
        public bool HasOverlapWithGroup(Group A)
        {
            return Cells.Any(c => A.Cells.Contains(c));
        }
    }
}

