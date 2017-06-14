using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to describe a single step in the solving process.
    /// </summary>
    class SolutionStep
    {
        #region Properties
        private bool _stepTaken;
        private string _description;

        /// <summary>
        /// Boolean value indicating if a step was actually taken.
        /// </summary>
        public bool StepTaken
        {
            get { return _stepTaken; }
            set { _stepTaken = value; }
        }

        /// <summary>
        /// String value describing the taken step.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with the indication if a step was taken, as parameter.
        /// </summary>
        /// <param name="stepTaken">Booleanvalue indicating if a step was taken to solve the sudoku</param>
        public SolutionStep(bool stepTaken)
        {
            _stepTaken = stepTaken;
        }

        /// <summary>
        /// Constructor with the indication if a step was taken, and the description of the step, as parameters.
        /// </summary>
        /// <param name="stepTaken">Booleanvalue indicating if a step was taken to solve the sudoku</param>
        /// <param name="description">The description of the step</param>
        public SolutionStep(bool stepTaken, string description): this(stepTaken)
        {
            _description = description;
        }

        #endregion
    }
}
