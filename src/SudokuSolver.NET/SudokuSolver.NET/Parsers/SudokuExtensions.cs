using SudokuSolver.NET.Engine;
using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Parsers;

internal static class SudokuExtensions
{
    extension(Sudoku sudoku)
    {
        public void Parse(string data)
        {
            if (data.Length != sudoku.CellCount)
                throw new SudokuParsingException("The serialized sudoku does not match the sudoku board.");

            sudoku.Reset();

            for (var cellId = 0; cellId < sudoku.CellCount; cellId++)
            {
                var cellValue = Parse(data[cellId], sudoku.CandidateCount);
                if (cellValue != Candidate.NotSet)
                {
                    sudoku.SetCell(cellId, cellValue);
                }
            }
        }
    }

    private static int Parse(char p, int candidateCount)
    {
        var v = Candidate.NotSet;
        switch (p)
        {
            case >= '0' and <= '9':
                v = p - '0';
                if (!(v >= 1 && v <= candidateCount))
                {
                    v = Candidate.NotSet;
                    break;
                }

                v--;

                break;
            case >= 'A' and <= 'Z':
                v = p - 'A' + 9;
                break;
        }

        return v;
    }
}