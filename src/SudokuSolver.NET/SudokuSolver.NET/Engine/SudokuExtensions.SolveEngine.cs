using SudokuSolver.NET.Engine.Techniques;
using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine;

internal static partial class SudokuExtensions
{
    extension(Sudoku sudoku)
    {
        public void Solve()
        {
            // TODO: Introduce options to choose techniques
            ITechnique[] techniques =
            [
                new NakedSingleTechnique(),
                new HiddenSingleTechnique(),
                //new NakedMultipleTechnique(),
                new HiddenMultipleTechnique()
            ];

            var solveStep = TrySolve(sudoku, techniques);
            while (solveStep != null)
            {
                // Apply step
                ApplySolveStep(sudoku, solveStep);

                // Try another solve
                solveStep = TrySolve(sudoku, techniques);
            }
        }
    }

    private static SolveStep? TrySolve(Sudoku sudoku, ITechnique[] techniques)
    {
        foreach (var technique in techniques)
        {
            var solveStep = technique.Solve(sudoku);
            if (solveStep != null)
                return solveStep;
        }
        return null;
    }

    private static void ApplySolveStep(Sudoku sudoku, SolveStep solveStep)
    {
        if (solveStep.Items.Length == 0)
        {
            return;
        }

        foreach (var item in solveStep.Items)
        {
            foreach (var cellId in item.CellIds)
            {
                switch (item.SolveStepType)
                {
                    case SolveStepItemType.CandidateConfirmation:
                        sudoku.SetCell(cellId, item.Value);
                        break;
                    case SolveStepItemType.CandidateRemoval:
                        sudoku.RemoveCandidate(cellId, item.Value);
                        break;
                }
            }

            Console.WriteLine($"{item.TechniqueName}: {item.Explanation}");
        }
    }
}
