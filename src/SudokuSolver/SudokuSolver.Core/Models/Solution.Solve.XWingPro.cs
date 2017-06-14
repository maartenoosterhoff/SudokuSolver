using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding an X-Wing or a Finned X-Wing.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryXWingPro()
        {
            SolutionStep step = new SolutionStep(false);

            for (int v = 0; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
                var listGroupA = new List<int>();
                var listGroupB = new List<int>();
                var groupsWithoutValue = FindGroupsWithoutValue(v);
                step = TryXWingPro_AddGroups(v, listGroupA, listGroupB, groupsWithoutValue);
            }

            return step;
        }

        private SolutionStep TryXWingPro_AddGroups(int v, List<int> listGroupA, List<int> listGroupB, bool[] groupsWithoutValue)
        {
            SolutionStep step = new SolutionStep(false);

            int lastGroupA = -1;
            int lastGroupB = -1;
            bool hasOverlap;

            if (listGroupA.Count > 0) lastGroupA = listGroupA[listGroupA.Count - 1];
            if (listGroupB.Count > 0) lastGroupB = listGroupB[listGroupB.Count - 1];

            // in een loop voor alle nietgebruikte groepen
            // 1) voeg group toe aan A
            // 2) voeg group toe aan B
            // 3) check voor xwing
            // 4) indien geen xwing
            // 5) zoek nog een group, en check voor finned xwing

            // Find another group for group A with no overlap with group A
            for (int newGroupA = 0; newGroupA < _groupList.Count && !step.StepTaken; newGroupA++) {
                if (groupsWithoutValue[newGroupA] && GroupCandidateCount(newGroupA, v) > 1) {
                    hasOverlap = false;
                    for (int i = 0; i < listGroupA.Count && !hasOverlap; i++) {
                        hasOverlap = _groupList[newGroupA].OverlapGroups.Layer[listGroupA[i]]
                            || (newGroupA == listGroupA[i]);
                    }
                    if (!hasOverlap) {
                        listGroupA.Add(newGroupA);
                        groupsWithoutValue[newGroupA] = false;

                        // Find another group for group B with no overlap with group B
                        for (int newGroupB = 0; newGroupB < _groupList.Count && !step.StepTaken; newGroupB++) {
                            if (groupsWithoutValue[newGroupB] && GroupCandidateCount(newGroupB, v) > 1) {
                                hasOverlap = false;
                                for (int i = 0; i < listGroupB.Count && !hasOverlap; i++) {
                                    hasOverlap = _groupList[newGroupB].OverlapGroups.Layer[listGroupB[i]]
                                        || (newGroupB == listGroupB[i]);
                                }
                                if (!hasOverlap) {
                                    listGroupB.Add(newGroupB);
                                    groupsWithoutValue[newGroupB] = false;

                                    // Make sure group A and group B have overlap
                                    hasOverlap = false;
                                    for (int a = 0; a < listGroupA.Count && !hasOverlap; a++)
                                        for (int b = 0; b < listGroupB.Count && !hasOverlap; b++)
                                            hasOverlap = _groupList[listGroupA[a]].OverlapGroups.Layer[listGroupB[b]];
                                    if (!hasOverlap) {
                                        if (listGroupA.Count > 1) {
                                            step = TryXWingPro_Check(v, listGroupA, listGroupB);
                                        }
                                        if (!step.StepTaken && listGroupA.Count < Candidate.PossibleCandidateCount - 1) {
                                            step = TryXWingPro_AddGroups(v, listGroupA, listGroupB, groupsWithoutValue);
                                        }
                                    }

                                    listGroupB.Remove(newGroupB);
                                    groupsWithoutValue[newGroupB] = true;
                                }
                            }
                        }
                        listGroupA.Remove(newGroupA);
                        groupsWithoutValue[newGroupA] = true;
                    }
                }
            }

            return step;
        }

        private SolutionStep TryXWingPro_Check(int v, List<int> listGroupA, List<int> listGroupB)
        {
            var step = new SolutionStep(false);

            var groupA = new BitLayer(Dimension, false);
            var groupB = new BitLayer(Dimension, false);
            var candidateLayer = CandidateAsBitLayer(v);
            var groupANames = string.Empty;
            var groupBNames = string.Empty;
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
            }

            return step;
        }
    }
}
