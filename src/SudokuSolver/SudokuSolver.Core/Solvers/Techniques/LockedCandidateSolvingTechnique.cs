using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class LockedCandidateSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                            from @group in proxy.SudokuBoard.Groups
                            where !proxy.GroupHasNumber(@group.Id, candidateValue)
                            from groupX in proxy.SudokuBoard.Groups
                            where groupX.Id != @group.Id
                            where @group.OverlapGroups[groupX.Id]
                            let solutionStep = TryLockedCandidateInternal(proxy, candidateValue, @group, groupX)
                            where solutionStep != null
                            select solutionStep;

            return solutions.FirstOrDefault();
        }

        private SolveStep TryLockedCandidateInternal(ISudokuBoardProxy proxy, int candidateValue, Group @group, Group groupX)
        {
            var candidateLayer = proxy.CandidateAsBitSet(candidateValue);
            var groupLayer = proxy.GroupAsBitSet(@group.Id);
            var groupXLayer = proxy.GroupAsBitSet(groupX.Id);
            // A -> Find the current group for current candidate
            var a = candidateLayer & groupLayer;
            // B -> Find the overlap between A and the other group
            // C -> Find the non-overlap of A and the over group, sort of A - group2
            var b = a & groupXLayer;
            var c = a & (!groupXLayer);
            if (!b.IsEmpty() && c.IsEmpty())
            {
                // D = group[groupId2] - B;
                var d = candidateLayer & groupXLayer & !groupLayer;
                if (!d.IsEmpty())
                {
                    //proxy.SetCandidateLayerWithBase(candidateValue, false, D);  // TODO: Remove this

                    var solveStep = new SolveStep
                    {
                        Items = new[]
                        {
                            new SolveStepItem
                            {
                                TechniqueName = "Locked Candidate",
                                SolveStepType = SolveStepItemType.CandidateRemoval,
                                CellIds = proxy.YieldCellIds(d),
                                Value = candidateValue,
                                Explanation = $"Found a Locked Candidate {Candidate.PrintValue(candidateValue)} in group {@group.Name}, removing candidate from cell(s) {proxy.YieldCellsDescription(d)}"
                            }
                        }
                    };

                    return solveStep;
                }
            }
            return null;
        }
    }
}
