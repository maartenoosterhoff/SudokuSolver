using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Engine
{
    public interface ISudokuSolverEngine
    {
        void Solve(ISudokuBoardProxy proxy);
    }
}
