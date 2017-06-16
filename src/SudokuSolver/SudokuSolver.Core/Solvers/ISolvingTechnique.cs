using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers
{
    public interface ISolvingTechnique
    {
        SolveStep Solve(ISudokuBoardProxy proxy);
    }
}
