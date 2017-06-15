namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to solve a sudoku.
    /// </summary>
    public class SudokuSolverEngine
    {
        #region Properties
        private Solution _solution;
        private string _solutionText;
        private string _solutionTextTotal;

        /// <summary>
        /// The newly added text explaining the solution to solve the sudoku.
        /// </summary>
        public string SolutionText
        {
            get {
                string result = _solutionText;
                _solutionText = string.Empty;
                return result; 
            }
        }

        /// <summary>
        /// The complete text explaining the solution to solve the sudoku.
        /// </summary>
        public string SolutionTextTotal
        {
            get { return _solutionTextTotal; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with the type of the sudoku as parameters.
        /// </summary>
        /// <param name="sudokuType">The type of the sudoku</param>
        public SudokuSolverEngine(SudokuType sudokuType)
        {
            _solution = new Solution(sudokuType);
            _solutionText = string.Empty;
            _solutionTextTotal = string.Empty;
            AddSolutionText("Sudoku Solver Engine v3.0 started\r\n");
        }

        /// <summary>
        /// Constructor with the type of the sudoku and the sudokustring as parameters.
        /// </summary>
        /// <param name="sudokuType">The type of the sudoku</param>
        /// <param name="sudokuString">The sudokustring to load</param>
        public SudokuSolverEngine(SudokuType sudokuType, string sudokuString)
            : this(sudokuType)
        {
            ParseSudokuString(sudokuString);
        }
        #endregion

        /// <summary>
        /// Parses a giving string as a sudoku. Can throw an error if the format is bad.
        /// </summary>
        /// <param name="sudokuString">The string representing the sudoku</param>
        public void ParseSudokuString(string sudokuString)
        {
            if (!_solution.ParseSudoku(sudokuString)) {
                throw new System.Exception("Error parsing sudokustring!");
            }
            else {
                AddSolutionText("Succesfully parsed the given string.\r\n");
            }
        }

        /// <summary>
        /// Visualizes a sudoku in textformat to the SolutionText property.
        /// </summary>
        public void Visualize()
        {
            AddSolutionText(_solution.Visualize());
        }

        /// <summary>
        /// Tries to solve the sudoku.
        /// </summary>
        public void Solve()
        {
            SolutionStep step = new SolutionStep(false);

            AddSolutionText("Solving started\r\n");

            do {
                step = _solution.Solve();
                //if (step.StepTaken)
                    AddSolutionText(step.Description);
            } while (step.StepTaken);

            _solution.CheckState();

            AddSolutionText("Solving stopped\r\n");
            AddSolutionText("Sudoku state: " + _solution.SudokuState.ToString() + "\r\n");
        }

        /// <summary>
        /// Adds a giving string to the sudoku explanation.
        /// </summary>
        /// <param name="text">The text to add</param>
        private void AddSolutionText(string text)
        {
            _solutionText += text;
            _solutionTextTotal += text;
        }
    }
}
