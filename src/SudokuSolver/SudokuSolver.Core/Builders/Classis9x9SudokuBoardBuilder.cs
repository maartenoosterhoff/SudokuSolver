using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Builders
{
    [SudokuType(SudokuType = SudokuType.Classic9by9)]
    public class Classis9x9SudokuBoardBuilder : ISudokuBoardBuilder
    {
        public SudokuBoard Build(SudokuType sudokuType)
        {
            var sudokuBoard = new SudokuBoard(
                SudokuType.Classic9by9,
                81,
                9,
                SudokuBoardBuilderHelper.CreateCells(9, 81)
            );

            sudokuBoard.Groups = SudokuBoardBuilderHelper.CreateDefaultGroups(sudokuBoard.Cells);

            SudokuBoardBuilderHelper.PopulateGroupOverlapData(sudokuBoard);

            return sudokuBoard;
        }
    }
}
