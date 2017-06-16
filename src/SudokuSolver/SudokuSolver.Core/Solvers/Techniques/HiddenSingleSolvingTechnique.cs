using System;
using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class HiddenSingleSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                            from @group in proxy.SudokuBoard.Groups
                            where !@group.Cells.Any(c => proxy.SudokuBoard.Cells[c].Value == candidateValue)
                            let applicableCells = @group.Cells.Where(c => proxy.SudokuBoard.Cells[c].Candidates[candidateValue]).ToArray()
                            where applicableCells.Length == 1
                            select new { lastCell = proxy.SudokuBoard.Cells[applicableCells.First()], candidateValue, @group };

            var solution = solutions.FirstOrDefault();
            if (solution != null)
            {
                return new SolveStep
                {
                    Items = new[]
                    {
                        new SolveStepItem
                        {
                            CellIds = new [] { solution.lastCell.ID },
                            SolveStepType = SolveStepItemType.CandidateConfirmation,
                            Value = solution.candidateValue,
                            TechniqueName = "Hidden Single",
                            Explanation = $"In group {solution.group.Name} the cell {solution.lastCell.Name} is the only cell for candidate {Candidate.PrintValue(solution.candidateValue)}."
                        }
                    }
                };
            }

            return null;
        }
    }
}
