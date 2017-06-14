using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding a Naked Double. This includes triples and higher.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryNakedDouble()
        {
            var step = new SolutionStep(false);

            for (var t = 2; t < Candidate.PossibleCandidateCount - 1 && !step.StepTaken; t++)
                for (var i = 0; i < _groupList.Count && !step.StepTaken; i++)
                    step = TryNakedDouble_Internal(t, i, 0, new BitLayer(Candidate.PossibleCandidateCount, false));

            return step;
        }

        private SolutionStep TryNakedDouble_Internal(int t, int groupId, int nextV, BitLayer b)
        {
            var step = new SolutionStep(false);

            if (t == b.Count()) {
                var selectedLayer = new BitLayer(Dimension, false);
                var unselectedLayer = new BitLayer(Dimension, false);
                for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                    if (b.Layer[v])
                        selectedLayer |= CandidateAsBitLayer(v);
                    else
                        unselectedLayer |= CandidateAsBitLayer(v);
                }
                var nakedDoubleLayer = selectedLayer & !unselectedLayer;
                nakedDoubleLayer = nakedDoubleLayer & GroupAsBitLayer(groupId);
                if (nakedDoubleLayer.Count() == t) {
                    var changesLayer = new BitLayer(Dimension, false);
                    BitLayer allChangesLayer;
                    for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                        if (b.Layer[v]) {
                            allChangesLayer = CandidateAsBitLayer(v) & GroupAsBitLayer(groupId);
                            allChangesLayer = allChangesLayer.SetWithBase(false, nakedDoubleLayer);
                            if (!allChangesLayer.IsEmpty()) {
                                SetCandidateLayerWithBase(v, false, allChangesLayer);
                                changesLayer = changesLayer | allChangesLayer;
                                step.StepTaken = true;
                            }
                        }
                    }
                    if (step.StepTaken) {
                        var n = string.Empty;
                        for (var v = 0; v < Candidate.PossibleCandidateCount; v++)
                            if (b.Layer[v]) {
                                if (!string.IsNullOrEmpty(n)) { n += "/"; }
                                n += Candidate.PrintValue(v);
                            }
                        step.Description = "Found a Naked Double " + n + " in group " + _groupList[groupId].Name + " - removing these candidates from cells " + YieldCells(changesLayer) + "\r\n";
                    }
                }
            }
            else {
                for (var v = nextV; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
                    if (!GroupHasNumber(groupId, v)) {
                        b.Layer[v] = true;
                        step = TryNakedDouble_Internal(t, groupId, v + 1, b);
                        b.Layer[v] = false;
                    }
                }
            }

            return step;
        }
    }
}
