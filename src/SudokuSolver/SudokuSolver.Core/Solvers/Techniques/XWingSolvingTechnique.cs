using SudokuSolver.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class XWingSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solutions = from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                            let groupsWithoutValue = (from @group in proxy.SudokuBoard.Groups
                                                      select !@group.CellIds.Any(c => proxy.SudokuBoard.Cells[c].Value == candidateValue)
                                                     ).ToArray()
                            from t in Enumerable.Range(2, proxy.SudokuBoard.CellCount)
                            where t < proxy.SudokuBoard.CellCount - 1
                            let solution = SolveInternal(proxy, candidateValue, t, 0, new List<int>(), new List<int>(), 0, groupsWithoutValue)
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private int GroupCandidateCount(ISudokuBoardProxy proxy, int groupId, int value)
        {
            return proxy.SudokuBoard.Groups[groupId].CellIds.Count(c => proxy.SudokuBoard.Cells[c].Candidates[value]);
        }

        private SolveStep SolveInternal(ISudokuBoardProxy proxy, int v, int t, int phase, List<int> listGroupA, List<int> listGroupB, int start, bool[] sourceGroupList)
        {
            switch (phase)
            {
                case 0:
                    {
                        if (listGroupA.Count < t)
                        {
                            for (var g = start; g < proxy.SudokuBoard.Groups.Length; g++)
                            {
                                if (sourceGroupList[g] && GroupCandidateCount(proxy, g, v) > 1)
                                {
                                    //var overlap = Enumerable.Range(0, listGroupA.Count).Any(i => proxy.SudokuBoard.Groups[g].OverlapGroups[listGroupA[i]] || (g == listGroupA[i]));

                                    var overlap = false;
                                    for (var i = 0; i < listGroupA.Count && !overlap; i++)
                                    {
                                        overlap = proxy.SudokuBoard.Groups[g].OverlapGroups[listGroupA[i]] || (g == listGroupA[i]);
                                    }
                                    if (!overlap)
                                    {
                                        listGroupA.Add(g);
                                        var solution = SolveInternal(proxy, v, t, phase, listGroupA, listGroupB, g + 1, sourceGroupList);
                                        listGroupA.Remove(g);
                                        if (solution != null)
                                        {
                                            return solution;
                                        }
                                    }
                                }
                            }
                        }
                        else if (listGroupA.Count == t)
                        {
                            // Remove the group A from the possible grouplist for group B.
                            foreach (var g in listGroupA)
                                sourceGroupList[g] = false;
                            // Try the next group
                            var solution = SolveInternal(proxy, v, t, phase + 1, listGroupA, listGroupB, 0, sourceGroupList);
                            // And put them back again
                            foreach (var g in listGroupA)
                                sourceGroupList[g] = true;

                            if (solution != null)
                            {
                                return solution;
                            }
                        }

                        break;
                    }
                case 1:
                    {
                        if (listGroupB.Count < t)
                        {
                            for (var g = start; g < proxy.SudokuBoard.Groups.Length; g++)
                            {
                                if (sourceGroupList[g] && GroupCandidateCount(proxy, g, v) > 1)
                                {
                                    var overlap = false;
                                    for (var i = 0; i < listGroupB.Count && !overlap; i++)
                                        overlap = proxy.SudokuBoard.Groups[g].OverlapGroups[listGroupB[i]] || (g == listGroupB[i]);
                                    if (!overlap)
                                    {
                                        listGroupB.Add(g);
                                        var solution = SolveInternal(proxy, v, t, phase, listGroupA, listGroupB, g + 1, sourceGroupList);
                                        listGroupB.Remove(g);
                                        if (solution != null)
                                        {
                                            return solution;
                                        }
                                    }
                                }
                            }
                        }
                        else if (listGroupA.Count == t)
                        {
                            var solution = SolveInternal(proxy, v, t, phase + 1, listGroupA, listGroupB, 0, sourceGroupList);
                            return solution;
                        }
                        break;
                    }
                case 2:
                    {
                        // Check for overlap between the two groups of groups
                        var overlap = false;
                        for (var a = 0; a < listGroupA.Count && !overlap; a++)
                        {
                            for (var b = 0; b < listGroupB.Count && !overlap; b++)
                            {
                                overlap = proxy.SudokuBoard.Groups[listGroupA[a]].OverlapGroups[listGroupB[b]];
                            }
                        }

                        //// Check for overlap between all the groups and all the other groups
                        //overlap = true;
                        //int aant;
                        //for (int a = 0; a < listGroupA.Count && overlap; a++) {
                        //    aant = 0;
                        //    for (int b = 0; b < listGroupB.Count; b++)
                        //        if (this._groupList[listGroupA[a]].OverlapGroups.Layer[listGroupB[b]])
                        //            aant++;
                        //    overlap = (aant >= 2);
                        //}
                        //for (int b = 0; b < listGroupB.Count && overlap; b++) {
                        //    aant = 0;
                        //    for (int a = 0; a < listGroupA.Count; a++)
                        //        if (this._groupList[listGroupB[b]].OverlapGroups.Layer[listGroupA[a]])
                        //            aant++;
                        //    overlap = (aant >= 2);
                        //}

                        if (overlap)
                        {
                            var groupA = new BitSet(proxy.SudokuBoard.CellCount, false);
                            var groupB = new BitSet(proxy.SudokuBoard.CellCount, false);
                            var candidateLayer = proxy.CandidateAsBitSet(v);
                            var groupANames = string.Empty;
                            var groupBNames = string.Empty;
                            foreach (var g in listGroupA)
                            {
                                groupA = groupA | proxy.GroupAsBitSet(g);
                                if (!string.IsNullOrEmpty(groupANames)) groupANames += "/";
                                groupANames += proxy.SudokuBoard.Groups[g].Name;
                            }
                            foreach (var g in listGroupB)
                            {
                                groupB = groupB | proxy.GroupAsBitSet(g);
                                if (!string.IsNullOrEmpty(groupBNames)) groupBNames += "/";
                                groupBNames += proxy.SudokuBoard.Groups[g].Name;
                            }
                            var a = candidateLayer & groupA;
                            var b = a & groupB;
                            var c = a & (!groupB);
                            if (!b.IsEmpty())
                            {
                                if (c.IsEmpty())
                                {
                                    var d = candidateLayer & groupB & !groupA;
                                    if (!d.IsEmpty())
                                    {
                                        var solveStep = new SolveStep
                                        {
                                            Items = new[]
                                            {
                                                new SolveStepItem
                                                {
                                                    TechniqueName="XWing",
                                                    SolveStepType=SolveStepItemType.CandidateRemoval,
                                                    Value = v,
                                                    Explanation = $"Found an X-Wing-{listGroupA.Count} with candidate {Candidate.PrintValue(v)} in groups {groupANames} and {groupBNames}, removing from cell(s) {proxy.YieldCellsDescription(d)}",
                                                    CellIds = proxy.BitSetToCellIdArray(d)
                                                }
                                            }
                                        };

                                        return solveStep;
                                        //SetCandidateLayerWithBase(v, false, D);
                                        //step.StepTaken = true;
                                        //step.Description = "Found an X-Wing-" + listGroupA.Count + " with candidate " + Candidate.PrintValue(v) + " in groups " + groupANames + " and " + groupBNames + " - removing from cell(s) " + YieldCells(D) + "\r\n";
                                    }
                                }
                                else
                                {
                                    // TODO: Decide if the Finned X-Wing should be in a separate method, because
                                    //       Finned X-Wing are now found before X-Wing if the X-Wing is located further
                                    //       in the grouplist.
                                    //////if (!step.StepTaken) {
                                    //////    for (int g = 0; g < this._groupList.Count; g++) {
                                    //////        if (sourceGroupList[g]) {
                                    //////            overlap = false;
                                    //////            for (int i = 0; i < listGroupA.Count && !overlap; i++) {
                                    //////                overlap = this._groupList[g].OverlapGroups.Layer[listGroupA[i]] &&
                                    //////                    this._groupList[g].OverlapGroups.Layer[listGroupB[i]];
                                    //////            }
                                    //////            if (overlap) {
                                    //////                BitLayer groupF = this.GroupAsBitLayer(g); // Fin-group
                                    //////                BitLayer E = C & !groupF;
                                    //////                BitLayer F = C & groupF;

                                    //////                BitLayer G = groupF & !(groupA & groupB);

                                    //////                if (E.IsEmpty() && !F.IsEmpty()) {
                                    //////                    BitLayer D = candidateLayer & groupF & groupB & !groupA;
                                    //////                    if (!D.IsEmpty()) {
                                    //////                        if (!D.IsEmpty()) {
                                    //////                            step.Description = string.Empty;
                                    //////                            //step.Description += this.Visualize();
                                    //////                            this.SetCandidateLayerWithBase(v, false, D);
                                    //////                            step.StepTaken = true;
                                    //////                            step.Description += "Found a Finned X-Wing-" + listGroupA.Count + " with candidate " + Candidate.printValue(v) + " in groups " + groupANames + " and " + groupBNames + " with Fin " + this._groupList[g].Name + " - removing from cell(s) " + this.yieldCells(D) + "\r\n";
                                    //////                            //step.Description += this.Visualize();
                                    //////                        }
                                    //////                    }
                                    //////                }
                                    //////            }
                                    //////        }
                                    //////    }
                                    //////}
                                }
                            }
                        }

                        break;
                    }
                default:

                    break;
            }

            return null;
        }
    }
}
