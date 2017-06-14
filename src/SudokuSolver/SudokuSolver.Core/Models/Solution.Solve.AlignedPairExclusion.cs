using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding an Aligned Pair Exclusion.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryAlignedPairExclusion()
        {
            var step = new SolutionStep(false);
            for (var groupA = 0; groupA < _groupList.Count && !step.StepTaken; groupA++)
            {
                for (var groupB = 0; groupB < _groupList.Count && !step.StepTaken; groupB++)
                {
                    if (_groupList[groupA].OverlapGroups.Layer[groupB])
                    {
                        var A = GroupAsBitLayer(groupA);
                        var B = GroupAsBitLayer(groupB);
                        var inLayer = A & B;
                        var outLayer = (A | B) & (!inLayer);
                        step = TryAlignedPairExclusion_Internal(inLayer, outLayer);
                    }
                }
            }
            return step;
        }

        private SolutionStep TryAlignedPairExclusion_Internal(BitLayer inLayer, BitLayer outLayer)
        {
            var step = new SolutionStep(false);

            //var cell1 = new List<int>();
            //var cell2 = new List<int>();

            var populations = TryAlignedPairExclusion_Populate(inLayer);
            var cell1 = populations.Item1;
            var cell2 = populations.Item2;
            var posMax = cell1.Length;
            for (var pos = 0; pos < posMax && !step.StepTaken; pos++)
            {
                // 1) Find all possible combinations of the values of the two paircells.
                var value1 = new List<int>();
                var value2 = new List<int>();
                var c1 = _cellList[cell1[pos]];
                var c2 = _cellList[cell2[pos]];
                for (var v1 = 0; v1 < Candidate.PossibleCandidateCount; v1++)
                    if (c1.Candidates[v1])
                        for (var v2 = 0; v2 < Candidate.PossibleCandidateCount; v2++)
                            if (c2.Candidates[v2])
                            {
                                value1.Add(v1);
                                value2.Add(v2);
                            }
                // Remove all combinations which are taken by the surrounding cells
                for (var lp = 0; lp < outLayer.Dimension; lp++)
                {
                    if (outLayer.Layer[lp])
                    {
                        var cell = _cellList[lp];
                        if (cell.CandidateCount() == 2)
                        {
                            var n1 = Candidate.NotSet;
                            var n2 = Candidate.NotSet;
                            for (int v = 0; v < Candidate.PossibleCandidateCount; v++)
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
                for (var i = 0; i < Candidate.PossibleCandidateCount; i++)
                {
                    if (c1.Candidates[i] && !value1.Contains(i))
                    {
                        _cellList[cell1[pos]].Candidates[i] = false;
                        step.StepTaken = true;
                        if (!string.IsNullOrEmpty(p1t)) p1t += ",";
                        p1t += Candidate.PrintValue(i);
                    }
                    if (c2.Candidates[i] && !value2.Contains(i))
                    {
                        _cellList[cell2[pos]].Candidates[i] = false;
                        if (!string.IsNullOrEmpty(p1t)) p2t += ",";
                        p2t += Candidate.PrintValue(i);
                    }
                }

                if (step.StepTaken)
                {
                    step.Description = "Found an Aligned Pair Exclusion in cells " + c1.Name + " and " + c2.Name + " - removing candidates ";
                    if (!string.IsNullOrEmpty(p1t))
                        step.Description += p1t + " from cell " + c1.Name;
                    if (!string.IsNullOrEmpty(p1t) && !string.IsNullOrEmpty(p2t))
                        step.Description += " and ";
                    if (!string.IsNullOrEmpty(p2t))
                        step.Description += p2t + " from cell " + c2.Name;
                    step.Description += "\r\n";
                }
            }

            return step;
        }

        private Tuple<int[], int[]> TryAlignedPairExclusion_Populate(BitLayer source)
        {
            var data = from c1 in Enumerable.Range(0, source.Dimension)
                       where source.Layer[c1] && _cellList[c1].CandidateCount() >= 2
                       from c2 in Enumerable.Range(c1 + 1, source.Dimension)
                       where c2 < source.Dimension  // TODO: calculate the iteration-length better so this criteria can be omitted, original for statement: for (int c2 = c1 + 1; c2 < source.Dimension; c2++)
                       where source.Layer[c2] && _cellList[c2].CandidateCount() >= 2
                       select new { c1, c2 };

            return Tuple.Create(
                data.Select(x => x.c1).ToArray(),
                data.Select(x => x.c2).ToArray()
            );

            //var cell1 = new List<int>();
            //var cell2 = new List<int>();
            //
            //for (int c1 = 0; c1 < source.Dimension; c1++)
            //{
            //    if (source.Layer[c1] && _cellList[c1].CandidateCount() >= 2)
            //    {
            //        for (int c2 = c1 + 1; c2 < source.Dimension; c2++)
            //        {
            //            if (source.Layer[c2] && _cellList[c2].CandidateCount() >= 2)
            //            {
            //                cell1.Add(c1);
            //                cell2.Add(c2);
            //            }
            //        }
            //    }
            //}
            //
            //return Tuple.Create(cell1, cell2);
        }
    }
}
