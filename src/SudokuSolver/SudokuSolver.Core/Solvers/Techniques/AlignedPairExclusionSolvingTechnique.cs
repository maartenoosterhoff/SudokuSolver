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
                            //let outLayer = (A | B) & (!inLayer)
                            let outLayer = A ^ B
                            let solution = SolveInternal(proxy, inLayer, outLayer)
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep SolveInternal(ISudokuBoardProxy proxy, BitSet inLayer, BitSet outLayer)
        {
            var applicableCells = from c1 in Enumerable.Range(0, inLayer.Size)
                                  where inLayer[c1] && proxy.SudokuBoard.Cells[c1].CurrentCandidateCount() >= 2
                                  from c2 in Enumerable.Range(c1 + 1, inLayer.Size)
                                  where c2 < inLayer.Size // TODO: calculate the iteration-length better so this criteria can be omitted, original for statement: for (int c2 = c1 + 1; c2 < source.Dimension; c2++)
                                  where inLayer[c2] && proxy.SudokuBoard.Cells[c2].CurrentCandidateCount() >= 2
                                  select new { cell1 = c1, cell2 = c2 };

            foreach (var cells in applicableCells)
            {
                //var populations = Populate(proxy, inLayer);
                //var cell1 = populations.Item1;
                //var cell2 = populations.Item2;
                //var posMax = cell1.Length;
                //for (var pos = 0; pos < posMax; pos++)
                //{

                //var cell1 = cells.cell1;
                //var cell2 = cells.cell2;


                // 1) Find all possible combinations of the values of the two paircells.
                var c1 = proxy.SudokuBoard.Cells[cells.cell1];
                var c2 = proxy.SudokuBoard.Cells[cells.cell2];
                //var c1 = proxy.SudokuBoard.Cells[cell1[pos]];
                //var c2 = proxy.SudokuBoard.Cells[cell2[pos]];
                //var cellCombos = from v1 in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                //                 where c1.Candidates[v1]
                //                 from v2 in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                //                 where c2.Candidates[v2]
                //                 select new { v1, v2 };

                var value1 = new List<int>();
                var value2 = new List<int>();
                for (var v1 = 0; v1 < proxy.SudokuBoard.CandidateCount; v1++)
                {
                    if (c1.Candidates[v1])
                    {
                        for (var v2 = 0; v2 < proxy.SudokuBoard.CandidateCount; v2++)
                        {
                            if (c2.Candidates[v2])
                            {
                                value1.Add(v1);
                                value2.Add(v2);
                            }
                        }
                    }
                }

                // Remove all combinations which are taken by the surrounding cells
                //var excludeCombos = (from lp in Enumerable.Range(0, outLayer.Size)
                //                     where outLayer[lp]
                //                     let cell = proxy.SudokuBoard.Cells[lp]
                //                     where cell.CurrentCandidateCount() == 2
                //                     let candidates = cell.Candidates.Select((x, i) => new { candidate = i, candidateValue = x }).Where(x => !x.candidateValue).Select(x => x.candidate).ToArray()
                //                     select candidates
                //                    )
                //                    .ToArray();

                //cellCombos = cellCombos
                //    .Where(x => !excludeCombos.Any(z => (x.v1 == z[0] && x.v2 == z[1]) || (x.v1 == z[1] && x.v2 == z[0]))
                //    ;

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
                            {
                                if ((value1[i] == n1 && value2[i] == n2) || (value1[i] == n2 && value2[i] == n1))
                                {
                                    value1.RemoveAt(i);
                                    value2.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

                // Update the paircells.
                var stepTaken = false;
                var solveStepItems = Enumerable.Range(0, proxy.SudokuBoard.CandidateCount).ToDictionary(i => i, i => new List<int>());
                for (var i = 0; i < proxy.SudokuBoard.CandidateCount; i++)
                {
                    if (c1.Candidates[i] && !value1.Contains(i))
                    {
                        solveStepItems[i].Add(c1.ID);// cell1);
                        //solveStepItems[i].Add(cell1[pos]);
                        stepTaken = true;
                    }
                    if (c2.Candidates[i] && !value2.Contains(i))
                    {
                        solveStepItems[i].Add(c2.ID);// cell2);
                        //solveStepItems[i].Add(cell2[pos]);
                    }
                }

                if (stepTaken)
                {
                    var items = from x in solveStepItems
                                where x.Value.Count > 0
                                select new SolveStepItem
                                {
                                    Value = x.Key,
                                    SolveStepType = SolveStepItemType.CandidateRemoval,
                                    CellIds = x.Value.ToArray(),
                                    TechniqueName = "Aligned Pair Exclusion",
                                    Explanation = $"Found an Aligned Pair Exclusion in cells {string.Join(",", x.Value.Select(c => proxy.SudokuBoard.Cells[c].Name))}, removing candidate {Candidate.PrintValue(x.Key)}"
                                };
                    var solveStep = new SolveStep()
                    {
                        Items = items.ToArray()
                    };
                    return solveStep;
                }
            }

            return null;
        }

        //private Tuple<int[], int[]> Populate(ISudokuBoardProxy proxy, BitSet source)
        //{
        //    var data = from c1 in Enumerable.Range(0, source.Size)
        //               where source[c1] && proxy.SudokuBoard.Cells[c1].CurrentCandidateCount() >= 2
        //               from c2 in Enumerable.Range(c1 + 1, source.Size)
        //               where c2 < source.Size // TODO: calculate the iteration-length better so this criteria can be omitted, original for statement: for (int c2 = c1 + 1; c2 < source.Dimension; c2++)
        //               where source[c2] && proxy.SudokuBoard.Cells[c2].CurrentCandidateCount() >= 2
        //               select new { c1, c2 };

        //    return Tuple.Create(
        //        data.Select(x => x.c1).ToArray(),
        //        data.Select(x => x.c2).ToArray()
        //    );
        //}
    }
}
