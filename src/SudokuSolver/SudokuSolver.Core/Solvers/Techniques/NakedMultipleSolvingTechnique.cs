using System;
using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    class NakedMultipleSolvingTechnique : ISolvingTechnique
    {
        public NakedMultipleSolvingTechnique(ISudokuBoardProxy proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            _proxy = proxy;
        }

        private readonly ISudokuBoardProxy _proxy;
        private readonly IDictionary<int, string> _titleMapper = new Dictionary<int, string>
        {
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

        public SolveStep Solve()
        {
            var solutions = from requiredCandidateCount in Enumerable.Range(2, _proxy.SudokuBoard.CandidateCount - 3)
                            from @group in _proxy.SudokuBoard.Groups
                            where requiredCandidateCount < _proxy.SudokuBoard.CandidateCount - 1   // TODO: Eliminate this by correctly calculate the range-size in the 1st 'from ... in ...'
                            let solution = SolveInternal(requiredCandidateCount, @group, 0, new BitLayer(_proxy.SudokuBoard.CandidateCount, false))
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep SolveInternal(int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitLayer b)
        {
            if (b.Count() == requiredCandidateCount)
            {
                var selectedLayer = new BitLayer(_proxy.SudokuBoard.CellCount, false);
                var unselectedLayer = new BitLayer(_proxy.SudokuBoard.CellCount, false);
                for (var v = 0; v < Candidate.PossibleCandidateCount; v++)
                {
                    if (b.Layer[v])
                        selectedLayer |= _proxy.CandidateAsBitLayer(v);
                    else
                        unselectedLayer |= _proxy.CandidateAsBitLayer(v);
                }

                var nakedDoubleLayer = selectedLayer & !unselectedLayer;
                nakedDoubleLayer = nakedDoubleLayer & _proxy.GroupAsBitLayer(@group.Id);
                if (nakedDoubleLayer.Count() == requiredCandidateCount)
                {
                    var changesLayer = new BitLayer(_proxy.SudokuBoard.CellCount, false);
                    BitLayer allChangesLayer;
                    var stepTaken = false;
                    for (var v = 0; v < Candidate.PossibleCandidateCount; v++)
                    {
                        if (b.Layer[v])
                        {
                            allChangesLayer = _proxy.CandidateAsBitLayer(v) & _proxy.GroupAsBitLayer(@group.Id);
                            allChangesLayer = allChangesLayer.SetWithBase(false, nakedDoubleLayer);
                            if (!allChangesLayer.IsEmpty())
                            {
                                _proxy.SetCandidateLayerWithBase(v, false, allChangesLayer);
                                changesLayer = changesLayer | allChangesLayer;
                                stepTaken = true;
                            }
                        }
                    }
                    if (stepTaken)
                    {
                        var solveStep = new SolveStep
                        {
                            Items = (from candidate in Enumerable.Range(0, _proxy.SudokuBoard.CandidateCount)
                                     where b.Layer[candidate]
                                     let cellIds = _proxy.YieldCellIds(changesLayer)
                                     select new SolveStepItem
                                     {
                                         CellIds = cellIds,
                                         SolveStepType = SolveStepItemType.CandidateRemoval,
                                         TechniqueName = _titleMapper[requiredCandidateCount],
                                         Value = candidate,
                                         Explanation = $"Found a {_titleMapper[requiredCandidateCount]} in group {@group.Name}, removing these candidates from cells {_proxy.YieldCellsDescription(changesLayer)}"
                                     }
                                    ).ToArray()
                        };

                        return solveStep;
                    }
                }

                return null;
            }

            // We haven't achieved the required-candidate-count yet.
            var potentialCandidates = from candidateValue in Enumerable.Range(nextCandidateValueStart, _proxy.SudokuBoard.CandidateCount)
                                      where candidateValue < _proxy.SudokuBoard.CandidateCount
                                      where !_proxy.GroupHasNumber(@group.Id, candidateValue)
                                      select candidateValue;

            foreach (var potentialCandidate in potentialCandidates)
            {
                b.Layer[potentialCandidate] = true;
                var solveStep = SolveInternal(requiredCandidateCount, @group, potentialCandidate + 1, b);
                b.Layer[potentialCandidate] = false;
                if (solveStep != null)
                {
                    return solveStep;
                }
            }

            return null;
        }
    }
}
