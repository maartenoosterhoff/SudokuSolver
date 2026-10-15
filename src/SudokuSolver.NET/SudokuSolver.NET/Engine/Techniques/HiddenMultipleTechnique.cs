using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class HiddenMultipleTechnique : ITechnique
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

    public SolveStep? Solve(Sudoku sudoku)
    {
        var solutions = from requiredCandidateCount in Enumerable.Range(2, sudoku.CandidateCount)
                        where requiredCandidateCount < sudoku.CandidateCount - 1
                        from @group in sudoku.Groups
                        let solution = SolveInternal(sudoku, requiredCandidateCount, @group, 0, new BitSet(sudoku.CandidateCount, false), new BitSet(sudoku.CellCount, true))
                        where solution != null
                        select solution;

        return solutions.FirstOrDefault();
    }

    private SolveStep? SolveInternal(Sudoku sudoku, int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitSet candidateIndicator, BitSet cellIndicator)
    {
        if (requiredCandidateCount == candidateIndicator.Count)
        {
            if (cellIndicator.Count != requiredCandidateCount)
            {
                return null;
            }

            var stepTaken = false;
            List<int> otherCandidates = [];
            for (var candidateValue = 0; candidateValue < sudoku.CandidateCount; candidateValue++)
            {
                if (!candidateIndicator[candidateValue])
                {
                    if (!(sudoku.CandidateAsBitSet(candidateValue) & cellIndicator).IsEmpty)
                    {
                        stepTaken = true;
                        //proxy.SetCandidateLayerWithBase(candidateValue, false, cellIndicator);    // TODO: Remove this

                        otherCandidates.Add(candidateValue);
                    }
                }
            }
            if (stepTaken)
            {
                var hiddenCandidates = (from c in Enumerable.Range(0, sudoku.CandidateCount)
                                        where candidateIndicator[c]
                                        select Candidate.PrintValue(c)).ToArray();
                var hiddenCandidatesPrint = string.Join("/", hiddenCandidates);
                var solveStep = new SolveStep([
                    ..
                    from candidate in otherCandidates
                    let cellIds = sudoku.YieldCellIds(cellIndicator)
                    select new SolveStepItem(
                        cellIds,
                        candidate,
                        SolveStepItemType.CandidateRemoval,
                        _titleMapper[requiredCandidateCount],
                        $"Found a {_titleMapper[requiredCandidateCount]} in group {@group.Name} for candidate(s) {hiddenCandidatesPrint}, removing candidate(s) {Candidate.PrintValue(candidate)} from cells {sudoku.YieldCellsDescription(cellIndicator)}")
                ]);
                return solveStep;
            }

            return null;
        }

        // We haven't achieved the required-candidate-count yet, so do the next recursive step
        var potentialCandidates = from candidateValue in Enumerable.Range(nextCandidateValueStart, sudoku.CandidateCount)
                                  where candidateValue < sudoku.CandidateCount
                                  where !sudoku.GroupHasNumber(@group.Id, candidateValue)
                                  select candidateValue;
        potentialCandidates = potentialCandidates.ToArray();

        foreach (var potentialCandidate in potentialCandidates)
        {
            var a = sudoku.CandidateAsBitSet(potentialCandidate) & sudoku.GroupAsBitSet(@group.Id);

            if (a.Count <= requiredCandidateCount && (cellIndicator & a).Count <= requiredCandidateCount)
            {
                candidateIndicator[potentialCandidate] = true;
                var solveStep = SolveInternal(sudoku, requiredCandidateCount, @group, potentialCandidate + 1, candidateIndicator, cellIndicator & a);
                candidateIndicator[potentialCandidate] = false;
                if (solveStep != null)
                    return solveStep;
            }
        }

        return null;
    }
}