using System.Collections.Concurrent;
using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Engine.Techniques;

internal sealed class AlignedPairExclusionTechnique : ITechnique
{
    public SolveStep? Solve(Sudoku sudoku)
    {
        var items = Iterator.Get(sudoku);
        var query =
            from item in items
            let solveStep = Calculator.Get(sudoku, item)
            where solveStep is not null
            select solveStep;

        return query.FirstOrDefault();
    }

    public record Iteration(BitSet InLayer, BitSet OutLayer, int Cell1Index, int Cell2Index);

    public static class Iterator
    {
        public static IEnumerable<Iteration> Get(Sudoku sudoku)
        {
            return

                // Iterate the groups
                from groupA in sudoku.Groups
                from groupB in sudoku.Groups
                where sudoku.Groups[groupA.Id].OverlapGroups[groupB.Id]
                let a = sudoku.GroupAsBitSet(groupA.Id)
                let b = sudoku.GroupAsBitSet(groupB.Id)
                let inLayer = a & b
                let outLayer = a ^ b

                // Iterate the cells in the groups-overlap
                from cell1Index in Enumerable.Range(0, inLayer.Size)
                where inLayer[cell1Index] && sudoku.Cells[cell1Index].CurrentCandidateCount() >= 2
                from cell2Index in Enumerable.Range(cell1Index + 1, inLayer.Size)
                where cell2Index < inLayer.Size
                where inLayer[cell2Index] && sudoku.Cells[cell2Index].CurrentCandidateCount() >= 2

                // Select iteration values
                select new Iteration(
                    inLayer,
                    outLayer,
                    cell1Index,
                    cell2Index
                );
        }
    }

    public static class Calculator
    {
        public static SolveStep? Get(Sudoku sudoku, Iteration iteration)
        {
            var c1 = sudoku.Cells[iteration.Cell1Index];
            var c2 = sudoku.Cells[iteration.Cell2Index];

            // Make a list of all candidate-combinations in the two cells.
            var value1 = new List<int>();
            var value2 = new List<int>();
            for (var v1 = 0; v1 < sudoku.CandidateCount; v1++)
            {
                if (!c1.Candidates[v1]) continue;
                for (var v2 = 0; v2 < sudoku.CandidateCount; v2++)
                {
                    if (!c2.Candidates[v2]) continue;
                    value1.Add(v1);
                    value2.Add(v2);
                }
            }

            // Loop through all cells in outerlayer
            var outLayer = iteration.OutLayer;
            for (var lp = 0; lp < outLayer.Size; lp++)
            {
                if (!outLayer[lp]) continue;
                
                // Only take cells with 2 candidates left
                var candidates = sudoku.Cells[lp].GetCurrentCandidates();
                if (candidates.Length != 2) continue;
                var n1 = candidates[0];
                var n2 = candidates[1];

                // If the 2 candidates are a combinations in the inner-layer, then remove that option in the inner-layer, since that would leave no possible value for the target-cell.
                for (var i = value1.Count - 1; i >= 0; i--)
                {
                    if ((value1[i] == n1 && value2[i] == n2) || (value1[i] == n2 && value2[i] == n1))
                    {
                        value1.RemoveAt(i);
                        value2.RemoveAt(i);
                    }
                }
            }

            // Check if any candidates are now completely removed from the combinations in the inner-layer. If so, we have a solve-step.
            var solveStepItems = new ConcurrentDictionary<int, List<int>>();
            for (var i = 0; i < sudoku.CandidateCount; i++)
            {
                if (c1.Candidates[i] && !value1.Contains(i))
                {
                    solveStepItems.GetOrAdd(i, []).Add(c1.Id);
                }
                if (c2.Candidates[i] && !value2.Contains(i))
                {
                    solveStepItems.GetOrAdd(i, []).Add(c2.Id);
                }
            }

            if (solveStepItems.IsEmpty)
            {
                return null;
            }

            var items = from x in solveStepItems
                select new SolveStepItem(
                    x.Value.ToArray(),
                    x.Key,
                    SolveStepItemType.CandidateRemoval,
                    "Aligned Pair Exclusion",
                    $"Found an Aligned Pair Exclusion in cells {string.Join(",", x.Value.Select(c => sudoku.Cells[c].Name))}, removing candidate {Candidate.PrintValue(x.Key)}");

            return new SolveStep([.. items]);

        }
    }
}