using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Builders
{
    [SudokuType(SudokuType = SudokuType.Classic9By9)]
    public class Classis9X9SudokuBoardBuilder : ISudokuBoardBuilder
    {
        public SudokuBoard Build(SudokuType sudokuType)
        {
            var sudokuBoard = new SudokuBoard(
                SudokuType.Classic9By9,
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
