using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding an X-Wing or a Finned X-Wing.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryXWing()
        {
            var step = new SolutionStep(false);
            bool[] groupWithoutValue;

            for (var v = 0; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
                groupWithoutValue = FindGroupsWithoutValue(v);
                for (var t = 2; t < Dimension - 1 && !step.StepTaken; t++) {
                    step = TryXWing_Internal(v, t, 0, new List<int>(), new List<int>(), 0, groupWithoutValue);//, new BitLayer(Solution.Dimension, false));
                }
            }

            return step;
        }

        private SolutionStep TryXWing_Internal(int v, int t, int phase, List<int> listGroupA, List<int> listGroupB, int start, bool[] sourceGroupList)//, BitLayer sourceBl)
        {
            var step = new SolutionStep(false);
            bool overlap;
            
            switch (phase) {
                case 0:
                    if (listGroupA.Count < t) {
                        for (var g = start; g < _groupList.Count && !step.StepTaken; g++) {
                            if (sourceGroupList[g] && GroupCandidateCount(g,v) > 1) {
                                overlap = false;
                                for (var i = 0; i < listGroupA.Count && !overlap; i++)
                                    overlap = _groupList[g].OverlapGroups.Layer[listGroupA[i]] || (g == listGroupA[i]);
                                if (!overlap) { // && !this._groupList[g].Name.StartsWith("B")) {
                                    listGroupA.Add(g);
                                    step = TryXWing_Internal(v, t, phase, listGroupA, listGroupB, g + 1, sourceGroupList);//, (sourceBl | tempBl));
                                    listGroupA.Remove(g);
                                }
                            }
                        }
                    }
                    else if (listGroupA.Count == t) {
                        // Remove the group A from the possible grouplist for group B.
                        foreach (var g in listGroupA)
                            sourceGroupList[g] = false;
                        // Try the next group
                        step = TryXWing_Internal(v, t, phase + 1, listGroupA, listGroupB, 0, sourceGroupList);//, new BitLayer(Solution.Dimension, false));
                        // And put them back again
                        foreach (var g in listGroupA)
                            sourceGroupList[g] = true;
                    }

                    break;

                case 1:
                    if (listGroupB.Count < t) {
                        for (var g = start; g < _groupList.Count && !step.StepTaken; g++) {
                            if (sourceGroupList[g] && GroupCandidateCount(g, v) > 1) {
                                overlap = false;
                                for (var i = 0; i < listGroupB.Count && !overlap; i++)
                                    overlap = _groupList[g].OverlapGroups.Layer[listGroupB[i]] || (g == listGroupB[i]);
                                if (!overlap) { // && !this._groupList[g].Name.StartsWith("B")) {
                                    listGroupB.Add(g);
                                    step = TryXWing_Internal(v, t, phase, listGroupA, listGroupB, g + 1, sourceGroupList);//, (sourceBl | tempBl));
                                    listGroupB.Remove(g);
                                }
                            }
                        }
                    }
                    else if (listGroupA.Count == t) {
                        step = TryXWing_Internal(v, t, phase + 1, listGroupA, listGroupB, 0, sourceGroupList);//, new BitLayer(Solution.Dimension, false));
                    }
                    break;

                case 2:
                    // Check for overlap between the two groups of groups
                    overlap = false;
                    for (var a = 0; a < listGroupA.Count && !overlap; a++)
                        for (var b = 0; b < listGroupB.Count && !overlap; b++)
                            overlap = _groupList[listGroupA[a]].OverlapGroups.Layer[listGroupB[b]];

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

                    if (overlap) {
                        var groupA = new BitLayer(Dimension, false);
                        var groupB = new BitLayer(Dimension, false);
                        var candidateLayer = CandidateAsBitLayer(v);
                        string groupANames = string.Empty;
                        string groupBNames = string.Empty;
                        foreach (var g in listGroupA) {
                            groupA = groupA | GroupAsBitLayer(g);
                            if (!string.IsNullOrEmpty(groupANames)) groupANames += "/";
                            groupANames += _groupList[g].Name;
                        }
                        foreach (var g in listGroupB) {
                            groupB = groupB | GroupAsBitLayer(g);
                            if (!string.IsNullOrEmpty(groupBNames)) groupBNames += "/";
                            groupBNames += _groupList[g].Name;
                        }
                        var A = candidateLayer & groupA;
                        var B = A & groupB;
                        var C = A & (!groupB);
                        if (!B.IsEmpty()) {
                            if (C.IsEmpty()) {
                                var D = candidateLayer & groupB & !groupA;
                                if (!D.IsEmpty()) {
                                    SetCandidateLayerWithBase(v, false, D);
                                    step.StepTaken = true;
                                    step.Description = "Found an X-Wing-" + listGroupA.Count + " with candidate " + Candidate.PrintValue(v) + " in groups " + groupANames + " and " + groupBNames + " - removing from cell(s) " + YieldCells(D) + "\r\n";
                                }
                            }
                            else {
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

                default:

                    break;
            }

            return step;
        }
    }
}
