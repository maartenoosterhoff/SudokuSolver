using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        /// <summary>
        /// Tries to take one step in the process of solving the sudoku.
        /// </summary>
        /// <param name="solveText">A referenced parameter </param>
        /// <returns></returns>
        public SolutionStep Solve()
        {
            SolutionStep step = new SolutionStep(false);
            
            if (!step.StepTaken) step = TryNakedSingle();
            if (!step.StepTaken) step = TryHiddenSingle();
            if (!step.StepTaken) step = TryLockedCandidate();
            if (!step.StepTaken) step = TryNakedDouble();
            if (!step.StepTaken) step = TryHiddenDouble();
            if (!step.StepTaken) step = TryAlignedPairExclusion();
            
            if (!step.StepTaken) {
                step = TryTableing();
                //step.StepTaken = false;
            }
            if (!step.StepTaken) step = TryXWingPro();
            if (!step.StepTaken) step = TryXWing(); // XWing as one of the lasts, because it is heavy...
            

            if (!step.StepTaken) _sudokuState = SudokuState.Unsolvable;
            
            return step;
        }
    }
}
