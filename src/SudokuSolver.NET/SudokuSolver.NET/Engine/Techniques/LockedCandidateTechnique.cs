using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class LockedCandidateTechnique : TechniqueBase<LockedCandidateTechnique.IteratorImpl, LockedCandidateTechnique.SolverImpl>
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

    public record Iteration(int CandidateValue, Group GroupA, Group GroupB);

    public class IteratorImpl
    {
        public IEnumerable<Iteration> Iterate(Sudoku sudoku)
        {
            return
                from candidateValue in Enumerable.Range(0, sudoku.CandidateCount)
                from groupA in sudoku.Groups
                where !sudoku.GroupHasCandidateValueSet(groupA.Id, candidateValue)
                from groupB in sudoku.Groups
                where !sudoku.GroupHasCandidateValueSet(groupB.Id, candidateValue)
                where groupB.Id != groupA.Id
                where groupA.OverlapGroups[groupB.Id]

                // Select iteration values
                select new Iteration(candidateValue, groupA, groupB);
        }
    }

    public class SolverImpl
    {
        public SolveStep? Solve(Sudoku sudoku, Iteration iteration)
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
            if (d.IsEmpty)
            {
                return null;
            }

            return new SolveStep(
            [
                new SolveStepItem(
                    sudoku.YieldCellIds(d),
                    iteration.CandidateValue,
                    SolveStepItemType.CandidateRemoval,
                    "Locked Candidate",
                    $"Found a Locked Candidate {Candidate.PrintValue(iteration.CandidateValue)} in group {iteration.GroupA.Name} vs group {iteration.GroupB.Name}, removing candidate from cell(s) {sudoku.YieldCellsDescription(d)}")
            ]);
        }
    }
}