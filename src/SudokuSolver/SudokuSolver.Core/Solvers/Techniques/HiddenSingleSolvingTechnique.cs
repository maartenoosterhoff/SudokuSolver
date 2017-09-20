using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class HiddenSingleSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = 
                // Try all candidates
                from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                // Try every group in the sudoku
                from @group in proxy.SudokuBoard.Groups
                // Where the group doesn't contain the candidate yet
                where @group.CellIds.All(c => proxy.SudokuBoard.Cells[c].Value != candidateValue)
                // Deduce which cells in the group are available to place the candidate
                let applicableCells = @group.CellIds.Where(c => proxy.SudokuBoard.Cells[c].Candidates[candidateValue]).ToArray()
                // Make sure there is only a single cell in the group that is available
                where applicableCells.Length == 1
                // Remember which cell it is
                let lastCell = proxy.SudokuBoard.Cells[applicableCells.First()]
                // Yes! Take that cell in the group, and place the candidate
                select new SolveStep
                {
                    Items = new[]
                    {
                        new SolveStepItem
                        {
                            CellIds = new [] { lastCell.Id },
                            SolveStepType = SolveStepItemType.CandidateConfirmation,
                            Value = candidateValue,
                            TechniqueName = "Hidden Single",
                            Explanation = $"In group {@group.Name} the cell {lastCell.Name} is the only cell for candidate {Candidate.PrintValue(candidateValue)}."
                        }
                    }
                };

            return solutions.FirstOrDefault();
        }
    }
}
