﻿using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class NakedSingleSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from cell in proxy.SudokuBoard.Cells
                            where cell.Value == Candidate.NotSet
                            where cell.CurrentCandidateCount() == 1
                            from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                            where cell.Candidates[candidateValue]
                            select new { cell, cell.ID, candidateValue };
            var solution = solutions.FirstOrDefault();
            if (solution != null)
            {
                return new SolveStep
                {
                    Items = new[]
                    {
                        new SolveStepItem
                        {
                            CellIds = new [] { solution.cell.ID },
                            SolveStepType = SolveStepItemType.CandidateConfirmation,
                            Value = solution.candidateValue,
                            TechniqueName = "Naked Single",
                            Explanation = $"The cell {solution.cell.Name} only has a single candidate left, value {Candidate.PrintValue(solution.candidateValue)}."
                        }
                    }
                };
            }

            return null;
        }
    }
}
