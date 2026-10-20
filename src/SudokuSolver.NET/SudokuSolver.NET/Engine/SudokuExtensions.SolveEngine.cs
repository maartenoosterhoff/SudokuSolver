using SudokuSolver.NET.Engine.Techniques;
using SudokuSolver.NET.Models;
using SudokuSolver.NET.Printing;

namespace SudokuSolver.NET.Engine;

internal static partial class SudokuExtensions
{
    extension(Sudoku sudoku)
    {
        public void Solve(Action<EngineOptions>? engineOptions = null)
        {
            var options = new EngineOptions();
            engineOptions?.Invoke(options);

            var techniques = options.Techniques.Build();

            SolveStep? solveStep;
            while ((solveStep = TrySolve(sudoku, techniques)) is not null)
            {
                // Apply step
                ApplySolveStep(sudoku, solveStep);

                if (options.DisplayBetweenSteps)
                {
                    Console.WriteLine(sudoku.Print());
                }
            }
        }
    }

    private static SolveStep? TrySolve(Sudoku sudoku, IEnumerable<ITechnique> techniques)
    {
        return techniques.Select(technique => technique.Solve(sudoku)).FirstOrDefault(x => x is not null);
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