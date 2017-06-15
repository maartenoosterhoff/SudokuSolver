using System.Linq;

namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to represent a single cell in a sudoku. Has properties like value and candidates.
    /// </summary>
    public class Cell
    {
        private int _value;

        public int ID { get; }

        /// <summary>
        /// The value of the cell. If set, all candidates of the cell are removed.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                for (int i = 0; i < Candidates.Length; i++)
                {
                    Candidates[i] = false;
                }
                //Candidates = Candidate.GetNoCandidates();
            }
        }

        /// <summary>
        /// The candidates of the cell. Read-only.
        /// </summary>
        public bool[] Candidates { get; private set; }
        //{
        //    get { return _candidates; }
        //}

        /// <summary>
        /// The name of the cell. Read-only.
        /// </summary>
        public string Name { get; }
        //{
        //    get { return _name; }
        //}

        public Cell(int id, string name, int value, bool[] candidates)
        {
            ID = id;
            Name = name;
            _value = value;
            Candidates = candidates;
        }

        /// <summary>
        /// Overrides the default ToString() function.
        /// </summary>
        /// <returns>Returns the printvalue of the cell.</returns>
        public override string ToString()
        {
            return Candidate.PrintValue(_value);
        }

        /// <summary>
        /// Returns the amounts of candidates left for the cell.
        /// </summary>
        /// <returns>The amount of candidates</returns>
        public int CurrentCandidateCount()
        {
            return Candidates.Count(x => x);
        }
    }
}
