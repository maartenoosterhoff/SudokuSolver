namespace SudokuSolver.Core.Solvers
{
    public interface ISolvingTechnique
    {
        SolveStep Solve();
    }
}
