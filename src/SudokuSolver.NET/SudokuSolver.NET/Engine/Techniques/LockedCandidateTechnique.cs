using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class LockedCandidateTechnique : ITechnique
{
    public SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Get(sudoku);
        var query =
            from item in items
            let candidateBitset = Calculator.GetRemovableCandidates(sudoku, item)
            where candidateBitset is not null

            select new SolveStep(
            [
                new SolveStepItem(
                    sudoku.YieldCellIds(candidateBitset),
                    item.CandidateValue,
                    SolveStepItemType.CandidateRemoval,
                    "Locked Candidate",
                    $"Found a Locked Candidate {Candidate.PrintValue(item.CandidateValue)} in group {item.GroupA.Name} vs group {item.GroupB.Name}, removing candidate from cell(s) {sudoku.YieldCellsDescription(candidateBitset)}")
            ]);

        return query.FirstOrDefault();
    }

    public record Iteration(int CandidateValue, Group GroupA, Group GroupB);

    public static class Iterator
    {
        public static IEnumerable<Iteration> Get(Sudoku sudoku)
        {
            return
                from candidateValue in Enumerable.Range(0, sudoku.CandidateCount)
                from groupA in sudoku.Groups
                where !sudoku.GroupHasNumber(groupA.Id, candidateValue)
                from groupB in sudoku.Groups
                where !sudoku.GroupHasNumber(groupB.Id, candidateValue)
                where groupB.Id != groupA.Id
                where groupA.OverlapGroups[groupB.Id]

                // Select iteration values
                select new Iteration(candidateValue, groupA, groupB);
        }
    }

    public static class Calculator
    {
        public static BitSet? GetRemovableCandidates(Sudoku sudoku, Iteration iteration)
        {
            var candidateLayer = sudoku.CandidateAsBitSet(iteration.CandidateValue);
            var bitsetGroupA = sudoku.GroupAsBitSet(iteration.GroupA.Id);
            var bitsetGroupB = sudoku.GroupAsBitSet(iteration.GroupB.Id);
            // A -> Find the current group for current candidate
            var a = candidateLayer & bitsetGroupA;
            // B -> Find the overlap between A and the other group
            // C -> Find the non-overlap of A and the over group, sort of A - group2
            var b = a & bitsetGroupB;
            var c = a & !bitsetGroupB;
            if (b.IsEmpty || !c.IsEmpty)
            {
                return null;
            }

            // D = group[groupId2] - B;
            var d = candidateLayer & bitsetGroupB & !bitsetGroupA;
            return d.IsEmpty ? null : d;
        }
    }
}