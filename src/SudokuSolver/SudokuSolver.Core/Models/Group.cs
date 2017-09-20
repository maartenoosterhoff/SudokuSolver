using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    public class Group
    {
        public int Id { get; }
        public string Name { get; }
        public int[] CellIds { get; }

        public BitSet OverlapGroups { get; set; }

        public Group(int id, string name, IEnumerable<int> cellIds)
        {
            Id = id;
            Name = name;
            CellIds = cellIds.ToArray();
        }

        public bool HasCell(int cellId)
        {
            return CellIds.Contains(cellId);
        }

    }
}

