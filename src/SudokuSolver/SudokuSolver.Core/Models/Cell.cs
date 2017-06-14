using System.Linq;

namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to represent a single cell in a sudoku. Has properties like value and candidates.
    /// </summary>
    public class Cell
    {
        #region Properties
        //private int _id;
        private int _value;
        //private bool[] _candidates;
        //private string _name = string.Empty;

        /// <summary>
        /// The ID of the cell. Read-only.
        /// </summary>
        public int ID { get; }
        //{
        //    get { return _id; }
        //    //set { _id = value; }
        //}

        /// <summary>
        /// The value of the cell. If set, all candidates of the cell are removed.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Candidates = Candidate.GetNoCandidates();
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
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with the ID and the name of the cell as parameters. The cell has no value yet,
        /// so all candidates are still available.
        /// </summary>
        /// <param name="id">The ID of the cell. Must be unique over all cells.</param>
        /// <param name="name">The name of the cell.</param>
        public Cell(int id, string name)
        {
            ID = id;
            _value = Candidate.NotSet;
            Candidates = Candidate.GetAllCandidates();
            Name = name;
        }

        /// <summary>
        /// Constructor with the ID, the name and the defaultvalue of the cell as parameters. The cell has a value,
        /// so no candidates are available.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        public Cell(int id, string name, int defaultValue)
        {
            ID = id;
            _value = defaultValue;
            Candidates = Candidate.GetNoCandidates();
            Name = name;
        }
        #endregion

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
        public int CandidateCount()
        {
            return Candidates.Take(Candidate.PossibleCandidateCount).Count(x => x);
            //int count = 0;
            //for (int i = 0; i < Candidate.PossibleCandidateCount; i++) {
            //    if (Candidates[i])
            //        count++;
            //}
            //return count;
        }
    }
}
