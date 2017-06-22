using System.Linq;
using System.Collections.Generic;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class HiddenMultipleSolvingTechnique : ISolvingTechnique
    {
        private readonly IDictionary<int, string> _titleMapper = new Dictionary<int, string>
        {
            { 2, "Hidden Double" },
            { 3, "Hidden Triple" },
            { 4, "Hidden Quadruple" },
            { 5, "Hidden Quintuple" },
            { 6, "Hidden Sextuple" },
            { 7, "Hidden Septuple" },
            { 8, "Hidden Octuple" },
            { 9, "Hidden Nonuple" },
            { 10, "Hidden Decuple" },
            { 11, "Hidden Undecuple" },
            { 12, "Hidden Duodecuple" },
            { 13, "Hidden Tredecuple" },
            { 14, "Hidden Quattordecuple" },
            { 15, "Hidden Quindecuple" }
        };
        private object otherCandidates;

        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from requiredCandidateCount in Enumerable.Range(2, proxy.SudokuBoard.CandidateCount)
                            where requiredCandidateCount < proxy.SudokuBoard.CandidateCount - 1
                            from @group in proxy.SudokuBoard.Groups
                            let solution = SolveInternal(proxy, requiredCandidateCount, @group, 0, new BitSet(proxy.SudokuBoard.CandidateCount, false), new BitSet(proxy.SudokuBoard.CellCount, true))
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep SolveInternal(ISudokuBoardProxy proxy, int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitSet candidateIndicator, BitSet cellIndicator)
        {
            if (requiredCandidateCount == candidateIndicator.Count())
            {
                if (cellIndicator.Count() == requiredCandidateCount)
                {
                    var stepTaken = false;
                    List<int> otherCandidates = null;
                    for (var candidateValue = 0; candidateValue < proxy.SudokuBoard.CandidateCount; candidateValue++)
                    {
                        if (!candidateIndicator[candidateValue])
                        {
                            if (!(proxy.CandidateAsBitSet(candidateValue) & cellIndicator).IsEmpty())
                            {
                                stepTaken = true;
                                //proxy.SetCandidateLayerWithBase(candidateValue, false, cellIndicator);    // TODO: Remove this

                                (otherCandidates = otherCandidates ?? new List<int>()).Add(candidateValue);
                            }
                        }
                    }
                    if (stepTaken)
                    {
                        var hiddenCandidates = (from c in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                                                where candidateIndicator[c]
                                                select Candidate.PrintValue(c)).ToArray();
                        var hiddenCandidatesPrint = string.Join("/", hiddenCandidates);
                        var solveStep = new SolveStep
                        {
                            Items = (from candidate in otherCandidates
                                     let cellIds = proxy.YieldCellIds(cellIndicator)
                                     select new SolveStepItem
                                     {
                                         CellIds = cellIds,
                                         SolveStepType = SolveStepItemType.CandidateRemoval,
                                         TechniqueName = _titleMapper[requiredCandidateCount],
                                         Value = candidate,
                                         Explanation = $"Found a {_titleMapper[requiredCandidateCount]} in group {@group.Name} for candidate(s) {hiddenCandidatesPrint}, removing candidate(s) {Candidate.PrintValue(candidate)} from cells {proxy.YieldCellsDescription(cellIndicator)}"
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
                var a = proxy.CandidateAsBitSet(potentialCandidate) & proxy.GroupAsBitSet(@group.Id);

                if (a.Count() <= requiredCandidateCount && (cellIndicator & a).Count() <= requiredCandidateCount)
                {
                    candidateIndicator[potentialCandidate] = true;
                    var solveStep = SolveInternal(proxy, requiredCandidateCount, @group, potentialCandidate + 1, candidateIndicator, cellIndicator & a);
                    candidateIndicator[potentialCandidate] = false;
                    if (solveStep != null)
                        return solveStep;
                }
            }

            return null;
        }
    }
}
