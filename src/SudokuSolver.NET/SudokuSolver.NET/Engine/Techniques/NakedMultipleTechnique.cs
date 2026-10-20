using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class NakedMultipleTechnique : TechniqueBase<NakedMultipleTechnique.IteratorImpl, NakedMultipleTechnique.SolverImpl>
{
    public override SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Iterate(sudoku);
        var query =
            from item in items
            let solveStep = Solver.Solve(sudoku, item)
            where solveStep is not null
            select solveStep;

        return query.FirstOrDefault();
    }

    public record Iteration(Sudoku Sudoku, Group Group, BitSet B);

    public class IteratorImpl
    {
        public IEnumerable<Iteration> Iterate(Sudoku sudoku)
        {
            return from requiredCandidateCount in Enumerable.Range(2, sudoku.CandidateCount)
                   where requiredCandidateCount < sudoku.CandidateCount - 1   // TODO: Eliminate this by correctly calculate the range-size in the 1st 'from ... in ...'
                   from @group in sudoku.Groups
                   from iteration in GetInternal(sudoku, requiredCandidateCount, @group, 0, new BitSet(sudoku.CandidateCount, false))
                   where iteration is not null
                   select iteration;
        }

        private static IEnumerable<Iteration?> GetInternal(Sudoku sudoku, int requiredCandidateCount, Group @group, int nextCandidateValueStart, BitSet b)
        {
            if (b.Count == requiredCandidateCount)
            {
                yield return new Iteration(sudoku, @group, b);
                yield break;
            }

            // We haven't achieved the required-candidate-count yet, so do the next recursive step
            var potentialCandidates = from candidateValue in Enumerable.Range(nextCandidateValueStart, sudoku.CandidateCount)
                                      where candidateValue < sudoku.CandidateCount
                                      where !sudoku.GroupHasCandidateValueSet(@group.Id, candidateValue)
                                      select candidateValue;

            foreach (var potentialCandidate in potentialCandidates)
            {
                b[potentialCandidate] = true;
                var iterations = GetInternal(sudoku, requiredCandidateCount, @group, potentialCandidate + 1, b);
                foreach (var iteration in iterations)
                {
                    yield return iteration;
                }
                b[potentialCandidate] = false;
            }

            yield break;
        }
    }

    public  class SolverImpl
    {
        private static readonly Dictionary<int, string> TitleMapper = new()
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

        public SolveStep? Solve(Sudoku sudoku, Iteration iteration)
        {
            var selectedLayer = new BitSet(sudoku.CellCount, false);
            var unselectedLayer = new BitSet(sudoku.CellCount, false);
            for (var v = 0; v < sudoku.CandidateCount; v++)
            {
                if (iteration.B[v])
                    selectedLayer |= sudoku.CandidateAsBitSet(v);
                else
                    unselectedLayer |= sudoku.CandidateAsBitSet(v);
            }

            var nakedDoubleLayer = selectedLayer & !unselectedLayer;
            nakedDoubleLayer &= sudoku.GroupAsBitSet(iteration.Group.Id);
            if (nakedDoubleLayer.Count == iteration.B.Count)
            {
                var stepTaken = false;
                var cellsToChange = new Dictionary<int, Tuple<int[], BitSet>>();
                for (var v = 0; v < sudoku.CandidateCount; v++)
                {
                    if (!iteration.B[v])
                    {
                        continue;
                    }
                    var allChangesLayer = sudoku.CandidateAsBitSet(v) & sudoku.GroupAsBitSet(iteration.Group.Id);
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
                        where iteration.B[candidate] && cellsToChange.ContainsKey(candidate)
                        let cellIds = cellsToChange[candidate].Item1
                        let changesLayer = cellsToChange[candidate].Item2
                        select new SolveStepItem(
                            cellIds,
                            candidate,
                            SolveStepItemType.CandidateRemoval,
                            TitleMapper[iteration.B.Count],
                            $"Found a {TitleMapper[iteration.B.Count]} in group {iteration.Group.Name}, removing candidate {Candidate.PrintValue(candidate)} from cells {sudoku.YieldCellsDescription(changesLayer)}")
                    ]);

                    return solveStep;
                }
            }

            return null;
        }
    }
}