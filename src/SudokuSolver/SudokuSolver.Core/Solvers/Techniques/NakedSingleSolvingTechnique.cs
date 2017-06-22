using System.Linq;
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
                            select new SolveStep
                            {
                                Items = new[]
                                {
                                    new SolveStepItem
                                    {
                                        CellIds = new [] { cell.ID },
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
