namespace SudokuSolver.Core.Models
{
    public static class Candidate
    {
        public const int NotSet = -1;

        public static string PrintValue(int value)
        {
            if (value <= 8)
                return $"{value + 1}";
            return $"{(char)((value - 9) + 'A')}";
        }
    }
}
