using System;
using System.Collections.Generic;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class HiddenMultipleSolvingTechnique : ISolvingTechnique
    {
        private readonly IDictionary<int, string> _titleMapper = new Dictionary<int, string>
        {
            { 2, "Hidden Double" },
            { 3, "Hidden Triple" },
            { 4, "Hidden Quadruple" },
            { 5, "Hidden Quintuple" },
            { 6, "Hidden Sextuple" },
            { 7, "Hidden Septuple" },
            { 8, "Hidden Octuple" },
            { 9, "Hidden Nonuple" },
            { 10, "Hidden Decuple" },
            { 11, "Hidden Undecuple" },
            { 12, "Hidden Duodecuple" },
            { 13, "Hidden Tredecuple" },
            { 14, "Hidden Quattordecuple" },
            { 15, "Hidden Quindecuple" }
        };

        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            return null;
        }
    }
}
