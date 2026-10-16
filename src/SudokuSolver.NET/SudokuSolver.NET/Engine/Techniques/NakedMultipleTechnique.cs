using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class NakedMultipleTechnique : ITechnique
{
    private static readonly IDictionary<int, string> TitleMapper = new Dictionary<int, string>
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

    public SolveStep? Solve(Sudoku sudoku)
    {
        var solutions = from requiredCandidateCount in Enumerable.Range(2, sudoku.CandidateCount)
                        where requiredCandidateCount < sudoku.CandidateCount - 1   // TODO: Eliminate this by correctly calculate the range-size in the 1st 'from ... in ...'
                        from @group in sudoku.Groups
                        let solution = SolveInternal(sudoku, requiredCandidateCount, @group, 0, new BitSet(sudoku.CandidateCount, false))
                        where solution != null
                        select solution;

        return solutions.FirstOrDefault();
    }

    private static SolveStep? SolveInternal(Sudoku sudoku, int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitSet b)
    {
        if (b.Count == requiredCandidateCount)
        {
            var selectedLayer = new BitSet(sudoku.CellCount, false);
            var unselectedLayer = new BitSet(sudoku.CellCount, false);
            for (var v = 0; v < sudoku.CandidateCount; v++)
            {
                if (b[v])
                    selectedLayer |= sudoku.CandidateAsBitSet(v);
                else
                    unselectedLayer |= sudoku.CandidateAsBitSet(v);
            }

            var nakedDoubleLayer = selectedLayer & !unselectedLayer;
            nakedDoubleLayer &= sudoku.GroupAsBitSet(group.Id);
            if (nakedDoubleLayer.Count == requiredCandidateCount)
            {
                var stepTaken = false;
                var cellsToChange = new Dictionary<int, Tuple<int[], BitSet>>();
                for (var v = 0; v < sudoku.CandidateCount; v++)
                {
                    if (!b[v])
                    {
                        continue;
                    }
                    var allChangesLayer = sudoku.CandidateAsBitSet(v) & sudoku.GroupAsBitSet(group.Id);
                    allChangesLayer = allChangesLayer.SetWithBase(false, nakedDoubleLayer);
                    if (!allChangesLayer.IsEmpty)
                    {
                        cellsToChange.Add(v, Tuple.Create(sudoku.BitSetToCellIdArray(allChangesLayer), allChangesLayer));
                        stepTaken = true;
                    }
                }

                if (stepTaken)
                {
                    var solveStep = new SolveStep([
                        ..
                        from candidate in Enumerable.Range(0, sudoku.CandidateCount)
                        where b[candidate] && cellsToChange.ContainsKey(candidate)
                        let cellIds = cellsToChange[candidate].Item1
                        let changesLayer = cellsToChange[candidate].Item2
                        select new SolveStepItem(
                            cellIds,
                            candidate,
                            SolveStepItemType.CandidateRemoval,
                            TitleMapper[requiredCandidateCount],
                            $"Found a {TitleMapper[requiredCandidateCount]} in group {@group.Name}, removing candidate {Candidate.PrintValue(candidate)} from cells {sudoku.YieldCellsDescription(changesLayer)}")
                    ]);

                    return solveStep;
                }
            }

            return null;
        }

        // We haven't achieved the required-candidate-count yet, so do the next recursive step
        var potentialCandidates = from candidateValue in Enumerable.Range(nextCandidateValueStart, sudoku.CandidateCount)
                                  where candidateValue < sudoku.CandidateCount
                                  where !sudoku.GroupHasNumber(@group.Id, candidateValue)
                                  select candidateValue;

        foreach (var potentialCandidate in potentialCandidates)
        {
            b[potentialCandidate] = true;
            var solveStep = SolveInternal(sudoku, requiredCandidateCount, @group, potentialCandidate + 1, b);
            b[potentialCandidate] = false;
            if (solveStep is not null)
            {
                return solveStep;
            }
        }

        return null;
    }

    public record Iteration(int RequiredCandidateCount, Group Group, BitSet Candidates);

    //public static class Iterator
    //{
    //    public static IEnumerable<Iteration> Get(Sudoku sudoku)
    //    {
    //        var groupsAndCandidates =
    //            from requiredCandidateCount in Enumerable.Range(2, sudoku.CandidateCount)
    //            where requiredCandidateCount <
    //                  sudoku.CandidateCount -
    //                  1 // TODO: Eliminate this by correctly calculate the range-size in the 1st 'from ... in ...'
    //            from @group in sudoku.Groups
    //            select new { requiredCandidateCount, @group };

    //        return
    //            // Try all cells on the board
    //            from cell in sudoku.Cells
    //            // Where the cell doesn't contain a value yet
    //            where cell.Value == Candidate.NotSet
    //            // Take only cells which have a single candidate left
    //            where cell.CurrentCandidateCount() == 1
    //            // Loop through all candidates
    //            from candidateValue in Enumerable.Range(0, sudoku.CandidateCount)
    //                // Select iteration values
    //            select new Iteration(cell, candidateValue);
    //    }
    //}
}