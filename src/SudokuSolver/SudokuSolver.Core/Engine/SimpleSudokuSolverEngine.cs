using System;
using SudokuSolver.Core.Models;
using SudokuSolver.Core.Solvers;

namespace SudokuSolver.Core.Engine
{
    public class SimpleSudokuSolverEngine : ISudokuSolverEngine
    {
        public SimpleSudokuSolverEngine(ISolvingTechnique[] techniques)
        {
            if (techniques == null)
                throw new ArgumentNullException(nameof(techniques));

            _techniques = techniques;
        }

        private readonly ISolvingTechnique[] _techniques;

        public void Solve(ISudokuBoardProxy proxy)
        {
            var solveStep = TrySolve(proxy);
            while (solveStep != null)
            {
                // Apply step
                ApplySolveStep(proxy, solveStep);

                // Try another solve
                solveStep = TrySolve(proxy);
            }
        }

        private SolveStep TrySolve(ISudokuBoardProxy proxy)
        {
            foreach (var technique in _techniques)
            {
                var solveStep = technique.Solve(proxy);
                if (solveStep != null)
                    return solveStep;
            }
            return null;
        }

        private void ApplySolveStep(ISudokuBoardProxy proxy, SolveStep solveStep)
        {
            if (solveStep == null || solveStep.Items == null)
                return;

            foreach (var item in solveStep.Items)
            {
                foreach (var cellId in item.CellIds)
                {
                    if (item.SolveStepType == SolveStepItemType.CandidateConfirmation)
                    {
                        proxy.SetCell(cellId, item.Value);
                    }
                    else if (item.SolveStepType == SolveStepItemType.CandidateRemoval)
                    {
                        proxy.RemoveCandidate(cellId, item.Value);

                    }
                }

                Console.WriteLine(item.Explanation);
            }
        }
    }
}
