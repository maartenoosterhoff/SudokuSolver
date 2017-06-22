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
                            where !@group.CellIds.Any(c => proxy.SudokuBoard.Cells[c].Value == candidateValue)
                            let applicableCells = @group.CellIds.Where(c => proxy.SudokuBoard.Cells[c].Candidates[candidateValue]).ToArray()
                            where applicableCells.Length == 1
                            let lastCell = proxy.SudokuBoard.Cells[applicableCells.First()]
                            select new SolveStep
                            {
                                Items = new[]
                                {
                                    new SolveStepItem
                                    {
                                        CellIds = new [] { lastCell.ID },
                                        SolveStepType = SolveStepItemType.CandidateConfirmation,
                                        Value = candidateValue,
                                        TechniqueName = "Hidden Single",
                                        Explanation = $"In group {group.Name} the cell {lastCell.Name} is the only cell for candidate {Candidate.PrintValue(candidateValue)}."
                                    }
                                }
                            };

            return solutions.FirstOrDefault();
        }
    }
}
