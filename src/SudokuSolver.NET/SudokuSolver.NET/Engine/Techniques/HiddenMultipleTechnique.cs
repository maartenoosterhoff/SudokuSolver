using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class HiddenMultipleTechnique : TechniqueBase<HiddenMultipleTechnique.IteratorImpl, HiddenMultipleTechnique.SolverImpl, HiddenMultipleTechnique.Iteration>
{
    public sealed record Iteration(Group Group, BitSet CandidateIndicator, BitSet CellIndicator);

    public sealed class IteratorImpl : IIterator<Iteration>
    {
        public IEnumerable<Iteration> Iterate(Sudoku sudoku)
        {
            return from requiredCandidateCount in Enumerable.Range(2, sudoku.CandidateCount)
                   where requiredCandidateCount < sudoku.CandidateCount - 1
                   from @group in sudoku.Groups
                   from iteration in GetInternal(sudoku, requiredCandidateCount, @group, 0, new BitSet(sudoku.CandidateCount, false), new BitSet(sudoku.CellCount, true))
                   where iteration is not null
                   select iteration;
        }

        private static IEnumerable<Iteration?> GetInternal(Sudoku sudoku, int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitSet candidateIndicator, BitSet cellIndicator)
        {
            if (requiredCandidateCount == candidateIndicator.Count)
            {
                if (cellIndicator.Count == requiredCandidateCount)
                {
                    yield return new Iteration(group, candidateIndicator, cellIndicator);
                }
                yield break;
            }

            // We haven't achieved the required-candidate-count yet, so do the next recursive step
            var potentialCandidates = from candidateValue in Enumerable.Range(nextCandidateValueStart, sudoku.CandidateCount)
                                      where candidateValue < sudoku.CandidateCount
                                      where !sudoku.GroupHasCandidateValueSet(@group.Id, candidateValue)
                                      select candidateValue;
            potentialCandidates = [.. potentialCandidates];

            foreach (var potentialCandidate in potentialCandidates)
            {
                var a = sudoku.CandidateAsBitSet(potentialCandidate) & sudoku.GroupAsBitSet(@group.Id);

                if (a.Count <= requiredCandidateCount && (cellIndicator & a).Count <= requiredCandidateCount)
                {
                    candidateIndicator[potentialCandidate] = true;
                    var iterations = GetInternal(sudoku, requiredCandidateCount, @group, potentialCandidate + 1, candidateIndicator, cellIndicator & a);
                    foreach (var iteration in iterations)
                    {
                        yield return iteration;
                    }
                    candidateIndicator[potentialCandidate] = false;
                }
            }

            yield break;
        }
    }

    public sealed class SolverImpl : ISolver<Iteration>
    {
        private static readonly Dictionary<int, string> TitleMapper = new()
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

        public SolveStep? Solve(Sudoku sudoku, Iteration iteration)
        {
            var stepTaken = false;
            List<int> otherCandidates = [];
            for (var candidateValue = 0; candidateValue < sudoku.CandidateCount; candidateValue++)
            {
                if (!iteration.CandidateIndicator[candidateValue])
                {
                    if (!(sudoku.CandidateAsBitSet(candidateValue) & iteration.CellIndicator).IsEmpty)
                    {
                        stepTaken = true;
                        otherCandidates.Add(candidateValue);
                    }
                }
            }
            if (stepTaken)
            {
                var hiddenCandidates = (from c in Enumerable.Range(0, sudoku.CandidateCount)
                                        where iteration.CandidateIndicator[c]
                                        select Candidate.PrintValue(c)).ToArray();
                var hiddenCandidatesPrint = string.Join("/", hiddenCandidates);
                var solveStep = new SolveStep([
                    ..
                    from candidate in otherCandidates
                    let cellIds = sudoku.YieldCellIds(iteration.CellIndicator)
                    select new SolveStepItem(
                        cellIds,
                        candidate,
                        SolveStepItemType.CandidateRemoval,
                        TitleMapper[iteration.CandidateIndicator.Count],
                        $"Found a {TitleMapper[iteration.CandidateIndicator.Count]} in group {iteration.Group.Name} for candidate(s) {hiddenCandidatesPrint}, removing candidate(s) {Candidate.PrintValue(candidate)} from cells {sudoku.YieldCellsDescription(iteration.CellIndicator)}")
                ]);
                return solveStep;
            }

            return null;
        }
    }
}