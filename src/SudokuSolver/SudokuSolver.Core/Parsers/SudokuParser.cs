using System;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Parsers
{
    public interface ISudokuParser
    {
        void ParseInto(ISudokuBoardProxy sudokuBoardProxy, string serializedSudoku);
    }

    public class SudokuParser : ISudokuParser
    {
        public void ParseInto(ISudokuBoardProxy sudokuBoardProxy, string serializedSudoku)
        {
            if (sudokuBoardProxy == null)
                throw new ArgumentNullException(nameof(sudokuBoardProxy));
            if (serializedSudoku == null)
                throw new ArgumentNullException(nameof(serializedSudoku));

            var sudokuBoard = sudokuBoardProxy.SudokuBoard;

            if (serializedSudoku.Length != sudokuBoard.CellCount)
                throw new SudokuParsingException("The serialized sudoku does not match the sudoku board.");

            sudokuBoardProxy.SudokuBoard.State = SudokuState.Reset;

            for (int cellId = 0; cellId < sudokuBoard.CellCount; cellId++)
            {
                var cellValue = Parse(serializedSudoku[cellId], sudokuBoard.CandidateCount);
                if (cellValue != Candidate.NotSet)
                {
                    sudokuBoardProxy.SetCell(cellId, cellValue);
                }
            }
        }

        private static int Parse(char p, int candidateCount)
        {
            int v = Candidate.NotSet;
            if (p >= '0' && p <= '9')
            {
                v = p - '0';
                if (!(v >= 1 && v <= candidateCount))
                    v = Candidate.NotSet;
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