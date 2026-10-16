namespace SudokuSolver.NET.Models;

internal static class Candidate
{
    public const int NotSet = -1;

    public static string PrintValue(int value)
    {
        return value <= 8 ? $"{value + 1}" : $"{(char)(value - 9 + 'A')}";
    }
}