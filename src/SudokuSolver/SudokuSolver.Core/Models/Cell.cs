using System.Linq;

namespace SudokuSolver.Core.Models
{
    //[DebuggerDisplay("{")]
    public class Cell
    {
        public int ID { get; }
        public int Value { get; private set; }

        public bool[] Candidates { get; private set; }
        public string Name { get; }

        public Cell(int id, string name, int value, bool[] candidates)
        {
            ID = id;
            Name = name;
            Value = value;
            Candidates = candidates;
        }

        public override string ToString()
        {
            //return Candidate.PrintValue(Value);

            return $"{Name}, {Value}, {string.Join("/", Candidates)}";
        }

        public int CurrentCandidateCount()
        {
            return Candidates.Count(x => x);
        }

        public void SetValue(int value)
        {
            Value = value;
            for (int i = 0; i < Candidates.Length; i++)
            {
                Candidates[i] = false;
            }
        }
    }
}
