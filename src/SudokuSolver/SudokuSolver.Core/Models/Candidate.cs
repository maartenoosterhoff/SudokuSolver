using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to handle all functions related to a candidate.
    /// Is static, so cannot be instanced. Is static because a value of a cell is just an int.
    /// </summary>
    static class Candidate
    {
        /// <summary>
        /// The possible amount of candidates for the current sudoku.
        /// </summary>
        public static int PossibleCandidateCount = -1;
        public const int NotSet = -1;

        /// <summary>
        /// Returns the printvalue of a candidate. 
        /// </summary>
        /// <param name="value">The candidatevalue</param>
        /// <returns>The printvalue of the candidate</returns>
        public static string PrintValue(int value)
        {
            if (value <= 8)
                return (value + 1).ToString();
            return ((char)((value - 9) + 'A')).ToString();
        }

        /// <summary>
        /// Returns an array of booleans, representing the candidates with all candidates signed as True,
        /// indicating they are available.
        /// </summary>
        /// <returns>An array of booleans</returns>
        public static bool[] GetAllCandidates()
        {
            return Enumerable.Range(0, PossibleCandidateCount).Select(x => true).ToArray();
            //var a = new bool[Candidate.PossibleCandidateCount];
            //for (var v = 0; v < Candidate.PossibleCandidateCount; v++)
            //{
            //    a[v] = true;
            //}
            //return a;
        }

        /// <summary>
        /// Returns an array of booleans, representing the candidates with all candidates signed as False,
        /// indicating they are unavailable.
        /// </summary>
        /// <returns>An array of booleans</returns>
        public static bool[] GetNoCandidates()
        {
            return Enumerable.Range(0, PossibleCandidateCount).Select(x => false).ToArray();
            //var a = new bool[Candidate.PossibleCandidateCount];
            //for (var v = 0; v < Candidate.PossibleCandidateCount; v++)
            //{
            //    a[v] = false;
            //}
            //return a;
        }

        /// <summary>
        /// Parses a printvalue of a candidate to an internal candidatevalue.
        /// </summary>
        /// <param name="p">The printvalue of a candidate</param>
        /// <returns>The internal candidatevalue</returns>
        public static int Parse(char p)
        {
            int v = NotSet;
            if (p >= '0' && p <= '9')
            {
                v = p - '0';
                if (!(v >= 1 && v <= PossibleCandidateCount))
                    v = NotSet;
                else
                    v--;
            }
            else if (p >= 'A' && p <= 'Z')
            {
                v = p - 'A' + 9;
            }

            return v;
        }
    }
}
