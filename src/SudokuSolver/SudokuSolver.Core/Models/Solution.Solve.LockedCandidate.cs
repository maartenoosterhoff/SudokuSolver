using System.Linq;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        private SolutionStep TryLockedCandidate()
        {
            var step = new SolutionStep(false);

            var items = from v in Enumerable.Range(0, Candidate.PossibleCandidateCount)
                        from groupId in Enumerable.Range(0, _groupList.Count)
                        where !GroupHasNumber(groupId, v)
                        from groupXId in Enumerable.Range(0, _groupList.Count)
                        where _groupList[groupId].OverlapGroups.Layer[groupXId]
                        let solutionStep = TryLockedCandidateInternal(v, groupId, groupXId)
                        where solutionStep != null
                        select solutionStep;
            step = items.FirstOrDefault() ?? step;
            return step;

            


                    
                    


            //for (var v = 0; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
            //    for (var groupId = 0; groupId < _groupList.Count && !step.StepTaken; groupId++) {
            //        if (!GroupHasNumber(groupId, v)) {
            //            for (var groupXId = 0; groupXId < _groupList.Count && !step.StepTaken; groupXId++) {
            //                if (_groupList[groupId].OverlapGroups.Layer[groupXId]) {
            //                    var candidateLayer = CandidateAsBitLayer(v);
            //                    var groupLayer = GroupAsBitLayer(groupId);
            //                    var groupXLayer = GroupAsBitLayer(groupXId);
            //                    // A -> Find the current group for current candidate
            //                    var A = candidateLayer & groupLayer;
            //                    // B -> Find the overlap between A and the other group
            //                    // C -> Find the non-overlap of A and the over group, sort of A - group2
            //                    var B = A & groupXLayer;
            //                    var C = A & (!groupXLayer);
            //                    if (!B.IsEmpty() && C.IsEmpty()) {
            //                        // D = group[groupId2] - B;
            //                        var D = candidateLayer & groupXLayer & !groupLayer;
            //                        if (!D.IsEmpty()) {
            //                            SetCandidateLayerWithBase(v, false, D);
            //                            step.StepTaken = true;
            //                            step.Description = "Found a Locked Candidate " + Candidate.printValue(v) + " in group " + _groupList[groupId].Name + " - removing from cell(s) " + yieldCells(D) + "\r\n";
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //return step;
        }

        private SolutionStep TryLockedCandidateInternal(int v, int groupId, int groupXId)
        {
            var candidateLayer = CandidateAsBitLayer(v);
            var groupLayer = GroupAsBitLayer(groupId);
            var groupXLayer = GroupAsBitLayer(groupXId);
            // A -> Find the current group for current candidate
            var A = candidateLayer & groupLayer;
            // B -> Find the overlap between A and the other group
            // C -> Find the non-overlap of A and the over group, sort of A - group2
            var B = A & groupXLayer;
            var C = A & (!groupXLayer);
            if (!B.IsEmpty() && C.IsEmpty())
            {
                // D = group[groupId2] - B;
                var D = candidateLayer & groupXLayer & !groupLayer;
                if (!D.IsEmpty())
                {
                    SetCandidateLayerWithBase(v, false, D);

                    var step = new SolutionStep(false);
                    step.StepTaken = true;
                    step.Description = "Found a Locked Candidate " + Candidate.PrintValue(v) + " in group " + _groupList[groupId].Name + " - removing from cell(s) " + YieldCells(D) + "\r\n";
                    return step;
                }
            }
            return null;
        }
    }
}
