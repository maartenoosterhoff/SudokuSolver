using System;
using System.Collections.Generic;
using System.Linq;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class AlignedPairExclusionSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from groupA in proxy.SudokuBoard.Groups
                            from groupB in proxy.SudokuBoard.Groups
                            where proxy.SudokuBoard.Groups[groupA.Id].OverlapGroups[groupB.Id]
                            let A = proxy.GroupAsBitSet(groupA.Id)
                            let B = proxy.GroupAsBitSet(groupB.Id)
                            let inLayer = A & B
                            let outLayer = (A | B) & (!inLayer)
                            let solution = SolveInternal(proxy, inLayer, outLayer)
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep SolveInternal(ISudokuBoardProxy proxy, BitSet inLayer, BitSet outLayer)
        {
            var populations = Populate(proxy, inLayer);
            var cell1 = populations.Item1;
            var cell2 = populations.Item2;
            var posMax = cell1.Length;
            for (var pos = 0; pos < posMax; pos++)
            {
                // 1) Find all possible combinations of the values of the two paircells.
                var value1 = new List<int>();
                var value2 = new List<int>();
                var c1 = proxy.SudokuBoard.Cells[cell1[pos]];
                var c2 = proxy.SudokuBoard.Cells[cell2[pos]];
                for (var v1 = 0; v1 < proxy.SudokuBoard.CandidateCount; v1++)
                    if (c1.Candidates[v1])
                        for (var v2 = 0; v2 < proxy.SudokuBoard.CandidateCount; v2++)
                            if (c2.Candidates[v2])
                            {
                                value1.Add(v1);
                                value2.Add(v2);
                            }
                // Remove all combinations which are taken by the surrounding cells
                for (var lp = 0; lp < outLayer.Size; lp++)
                {
                    if (outLayer[lp])
                    {
                        var cell = proxy.SudokuBoard.Cells[lp];
                        if (cell.CurrentCandidateCount() == 2)
                        {
                            var n1 = Candidate.NotSet;
                            var n2 = Candidate.NotSet;
                            for (int v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
                            {
                                if (cell.Candidates[v])
                                    if (n1 == Candidate.NotSet)
                                        n1 = v;
                                    else
                                        n2 = v;
                            }
                            for (var i = value1.Count - 1; i >= 0; i--)
                                if ((value1[i] == n1 && value2[i] == n2) || (value1[i] == n2 && value2[i] == n1))
                                {
                                    value1.RemoveAt(i);
                                    value2.RemoveAt(i);
                                }
                        }
                    }
                }

                // Update the paircells.
                var p1t = string.Empty;
                var p2t = string.Empty;
                var stepTaken = false;
                var solveStepItems = Enumerable.Range(0, proxy.SudokuBoard.CandidateCount).ToDictionary(i => i, i => new SolveStepItem() { Value = i, TechniqueName = "Aligned Pair Exclusion", SolveStepType = SolveStepItemType.CandidateRemoval });
                for (var i = 0; i < proxy.SudokuBoard.CandidateCount; i++)
                {
                    if (c1.Candidates[i] && !value1.Contains(i))
                    {
                        var cellIds = (solveStepItems[i].CellIds ?? new int[0]).ToList();
                        cellIds.Add(cell1[pos]);
                        solveStepItems[i].CellIds = cellIds.ToArray();
                        //proxy.SudokuBoard.Cells[cell1[pos]].Candidates[i] = false;
                        stepTaken = true;
                        //if (!string.IsNullOrEmpty(p1t)) p1t += ",";
                        //p1t += Candidate.PrintValue(i);
                    }
                    if (c2.Candidates[i] && !value2.Contains(i))
                    {
                        var cellIds = (solveStepItems[i].CellIds ?? new int[0]).ToList();
                        cellIds.Add(cell2[pos]);
                        solveStepItems[i].CellIds = cellIds.ToArray();
                        //proxy.SudokuBoard.Cells[cell2[pos]].Candidates[i] = false;
                        //if (!string.IsNullOrEmpty(p1t)) p2t += ",";
                        //p2t += Candidate.PrintValue(i);
                    }
                }

                if (stepTaken)
                {
                    var items = solveStepItems.Select(x => x.Value).Where(x => x.CellIds != null).ToArray();
                    foreach (var item in items)
                    {
                        item.Explanation = $"Found an Aligned Pair Exclusion in cells {string.Join(",", item.CellIds.Select(x => proxy.SudokuBoard.Cells[x].Name))}, removing candidate {item.Value}";
                    }
                    var solveStep = new SolveStep()
                    {
                        Items = items
                    };
                    //step.Description = "Found an Aligned Pair Exclusion in cells " + c1.Name + " and " + c2.Name + " - removing candidates ";
                    //if (!string.IsNullOrEmpty(p1t))
                    //    step.Description += p1t + " from cell " + c1.Name;
                    //if (!string.IsNullOrEmpty(p1t) && !string.IsNullOrEmpty(p2t))
                    //    step.Description += " and ";
                    //if (!string.IsNullOrEmpty(p2t))
                    //    step.Description += p2t + " from cell " + c2.Name;
                    //step.Description += "\r\n";

                    return solveStep;
                }
            }

            return null;
        }

        private Tuple<int[], int[]> Populate(ISudokuBoardProxy proxy, BitSet source)
        {
            var data = from c1 in Enumerable.Range(0, source.Size)
                       where source[c1] && proxy.SudokuBoard.Cells[c1].CurrentCandidateCount() >= 2
                       from c2 in Enumerable.Range(c1 + 1, source.Size)
                       where c2 < source.Size // TODO: calculate the iteration-length better so this criteria can be omitted, original for statement: for (int c2 = c1 + 1; c2 < source.Dimension; c2++)
                       where source[c2] && proxy.SudokuBoard.Cells[c2].CurrentCandidateCount() >= 2
                       select new { c1, c2 };

            return Tuple.Create(
                data.Select(x => x.c1).ToArray(),
                data.Select(x => x.c2).ToArray()
            );
        }
    }
}
