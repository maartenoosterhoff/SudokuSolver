using System.Linq;

namespace SudokuSolver.Core.Models
{
    public class Cell
    {
        public int Id { get; }
        public int Value { get; private set; }

        public bool[] Candidates { get; }
        public string Name { get; }

        public Cell(int id, string name, int value, bool[] candidates)
        {
            Id = id;
            Name = name;
            Value = value;
            Candidates = candidates;
        }

        public override string ToString()
        {
            return $"{Name}, {Value}, {string.Join("/", Candidates)}";
        }

        public int CurrentCandidateCount()
        {
            return Candidates.Count(x => x);
        }

        public void SetValue(int value)
        {
            Value = value;
            for (var i = 0; i < Candidates.Length; i++)
            {
                Candidates[i] = false;
            }
        }
    }
}
