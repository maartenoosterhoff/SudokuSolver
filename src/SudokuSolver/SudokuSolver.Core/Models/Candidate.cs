using System.Linq;

namespace SudokuSolver.Core.Models
{
    static class Candidate
    {
        public static int PossibleCandidateCount = -1;
        public const int NotSet = -1;

        public static string PrintValue(int value)
        {
            if (value <= 8)
                return $"{value + 1}";
            return $"{(char)((value - 9) + 'A')}";
        }
    }
}
