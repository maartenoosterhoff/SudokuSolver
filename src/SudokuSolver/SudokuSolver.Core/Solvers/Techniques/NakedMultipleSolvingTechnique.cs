using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class NakedMultipleSolvingTechnique : ISolvingTechnique
    {
        private readonly IDictionary<int, string> _titleMapper = new Dictionary<int, string>
        {
            { 2, "Naked Double" },
            { 3, "Naked Triple" },
            { 4, "Naked Quadruple" },
            { 5, "Naked Quintuple" },
            { 6, "Naked Sextuple" },
            { 7, "Naked Septuple" },
            { 8, "Naked Octuple" },
            { 9, "Naked Nonuple" },
            { 10, "Naked Decuple" },
            { 11, "Naked Undecuple" },
            { 12, "Naked Duodecuple" },
            { 13, "Naked Tredecuple" },
            { 14, "Naked Quattordecuple" },
            { 15, "Naked Quindecuple" }
        };

        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from requiredCandidateCount in Enumerable.Range(2, proxy.SudokuBoard.CandidateCount)
                            where requiredCandidateCount < proxy.SudokuBoard.CandidateCount - 1   // TODO: Eliminate this by correctly calculate the range-size in the 1st 'from ... in ...'
                            from @group in proxy.SudokuBoard.Groups
                            let solution = SolveInternal(proxy, requiredCandidateCount, @group, 0, new BitSet(proxy.SudokuBoard.CandidateCount, false))
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep SolveInternal(ISudokuBoardProxy proxy, int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitSet b)
        {
            if (b.Count() == requiredCandidateCount)
            {
                var selectedLayer = new BitSet(proxy.SudokuBoard.CellCount, false);
                var unselectedLayer = new BitSet(proxy.SudokuBoard.CellCount, false);
                for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
                {
                    if (b[v])
                        selectedLayer |= proxy.CandidateAsBitSet(v);
                    else
                        unselectedLayer |= proxy.CandidateAsBitSet(v);
                }

                var nakedDoubleLayer = selectedLayer & !unselectedLayer;
                nakedDoubleLayer = nakedDoubleLayer & proxy.GroupAsBitSet(@group.Id);
                if (nakedDoubleLayer.Count() == requiredCandidateCount)
                {
                    var changesLayer = new BitSet(proxy.SudokuBoard.CellCount, false);
                    BitSet allChangesLayer;
                    var stepTaken = false;
                    for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
                    {
                        if (b[v])
                        {
                            allChangesLayer = proxy.CandidateAsBitSet(v) & proxy.GroupAsBitSet(@group.Id);
                            allChangesLayer = allChangesLayer.SetWithBase(false, nakedDoubleLayer);
                            if (!allChangesLayer.IsEmpty())
                            {
                                proxy.SetCandidateLayerWithBase(v, false, allChangesLayer);
                                changesLayer = changesLayer | allChangesLayer;
                                stepTaken = true;
                            }
                        }
                    }
                    if (stepTaken)
                    {
                        var solveStep = new SolveStep
                        {
                            Items = (from candidate in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                                     where b[candidate]
                                     let cellIds = proxy.YieldCellIds(changesLayer)
                                     select new SolveStepItem
                                     {
                                         CellIds = cellIds,
                                         SolveStepType = SolveStepItemType.CandidateRemoval,
                                         TechniqueName = _titleMapper[requiredCandidateCount],
                                         Value = candidate,
                                         Explanation = $"Found a {_titleMapper[requiredCandidateCount]} in group {@group.Name}, removing these candidates from cells {proxy.YieldCellsDescription(changesLayer)}"
                                     }
                                    ).ToArray()
                        };

                        return solveStep;
                    }
                }

                return null;
            }

            // We haven't achieved the required-candidate-count yet, so do the next recursive step
            var potentialCandidates = from candidateValue in Enumerable.Range(nextCandidateValueStart, proxy.SudokuBoard.CandidateCount)
                                      where candidateValue < proxy.SudokuBoard.CandidateCount
                                      where !proxy.GroupHasNumber(@group.Id, candidateValue)
                                      select candidateValue;

            foreach (var potentialCandidate in potentialCandidates)
            {
                b[potentialCandidate] = true;
                var solveStep = SolveInternal(proxy, requiredCandidateCount, @group, potentialCandidate + 1, b);
                b[potentialCandidate] = false;
                if (solveStep != null)
                {
                    return solveStep;
                }
            }

            return null;
        }
    }
}
