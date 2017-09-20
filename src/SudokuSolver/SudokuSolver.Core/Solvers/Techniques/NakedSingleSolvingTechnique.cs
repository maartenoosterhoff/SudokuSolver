using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class NakedSingleSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = 
                // Try all cells on the board
                from cell in proxy.SudokuBoard.Cells
                // Where the cell doesn't contain a value yet
                where cell.Value == Candidate.NotSet
                // Take only cells which have a single candidate left
                where cell.CurrentCandidateCount() == 1
                // Loop through all candidates
                from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                // Select the candidate that is left
                where cell.Candidates[candidateValue]
                // Yes! Take the candidate for that cell, and place the candidate
                select new SolveStep
                {
                    Items = new[]
                    {
                        new SolveStepItem
                        {
                            CellIds = new [] { cell.Id },
                            SolveStepType = SolveStepItemType.CandidateConfirmation,
                            Value = candidateValue,
                            TechniqueName = "Naked Single",
                            Explanation = $"The cell {cell.Name} only has a single candidate left, value {Candidate.PrintValue(candidateValue)}."
                        }
                    }
                };

            return solutions.FirstOrDefault();
        }
    }
}
