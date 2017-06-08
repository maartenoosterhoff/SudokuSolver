using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core.Model
{
    public class Group
    {
        public string Name { get; }
        public List<Cell> Cells { get; }
    }
}
