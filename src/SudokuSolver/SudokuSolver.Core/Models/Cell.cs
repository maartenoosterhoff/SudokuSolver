using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    public class Cell
    {
        public int Id { get; }
        public int Value { get; private set; }

        private readonly List<bool> _candidates;
        public IReadOnlyList<bool> Candidates => _candidates.AsReadOnly();
        public string Name { get; }

        public Cell(int id, string name, int value, bool[] candidates)
        {
            Id = id;
            Name = name;
            Value = value;
            _candidates = candidates.ToList();
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
            for (var i = 0; i < _candidates.Count; i++)
            {
                _candidates[i] = false;
            }
        }

        public void RemoveCandidate(int value)
        {
            if (_candidates[value])
            {
                _candidates[value] = false;
            }
        }
    }
}
