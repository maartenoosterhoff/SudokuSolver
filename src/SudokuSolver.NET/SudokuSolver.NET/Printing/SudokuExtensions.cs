using SudokuSolver.NET.Extensions;
using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Printing;

internal static class SudokuExtensions
{
    private static readonly Dictionary<int, char> CandidateDisplayValues = Enumerable.Range(0, 16).ToDictionary(
        i => i,
        i => (char)i <= 8 ? (char)(i + 1 + '0') : (char)(i - 9 + 'A')
    );

    extension(Sudoku sudoku)
    {
        public string Print()
        {
            return sudoku.SudokuType switch
            {
                SudokuType.Classic9By9 => Print9X9(sudoku),
                _ => throw new NotSupportedException()
            };
        }
    }

    private static string Print9X9(Sudoku sudoku)
    {
        var result = "\r\n";
        var max = sudoku.Cells.Max(x => x.CurrentCandidateCount());

        if (max < 4)
            max = 4;
        if (max == 9)
            max = 8;

        var bar = string.Empty.PadRight(9 * max + 8, '-');

        for (var r = 0; r < 9; r++)
        {
            var line = string.Empty;
            if (r.In(3, 6))
                result += bar + "\r\n";
            for (var c = 0; c < 9; c++)
            {
                if (c.In(3, 6))
                {
                    line += "|";
                }
                else if (c != 0)
                {
                    line += " ";
                }
                var cell = sudoku.Cells[(r * 9) + c];
                if (cell.Value != -1)
                {
                    line += CandidateDisplayValues[cell.Value];
                    for (var i = 0; i < max - 1; i++)
                        line += " ";
                }
                else
                {
                    var currentCandidateCount = cell.CurrentCandidateCount();
                    if (currentCandidateCount == 9)
                    {
                        line += "all";
                        for (var i = 0; i < max - 3; i++)
                            line += " ";
                    }
                    else
                    {
                        var charCount = 0;
                        for (var i = 0; i < sudoku.CandidateCount; i++)
                        {
                            if (cell.Candidates[i])
                            {
                                charCount++;
                                line += CandidateDisplayValues[i];
                            }
                        }
                        if (currentCandidateCount == 1)
                        {
                            charCount++;
                            line += "*";
                        }

                        for (var i = 0; i < (max - charCount); i++)
                        {
                            line += " ";
                        }
                    }
                }
            }
            result += line + "\r\n";
        }

        return result;
    }
}