using System.Linq;
using SudokuSolver.Core.Models;
using System;
using System.Collections.Generic;

namespace SudokuSolver.Core.Solvers.Techniques
{
    public class TableingItem
    {
        public int CellId { get; set; }
        public int CandidateValue { get; set; }
    }

    public class TableingSolvingTechnique : ISolvingTechnique
    {
        public SolveStep Solve(ISudokuBoardProxy proxy)
        {
            var solution = TryTableingByCell(proxy);
            if (solution != null)
            {
                return solution;
            }

            solution = TryTableingByGroup(proxy);
            return solution;
        }

        private SolveStep TryTableingByCell(ISudokuBoardProxy proxy)
        {
            var cellIds = from c in proxy.SudokuBoard.Cells
                          let candidateCount = c.CurrentCandidateCount()
                          where candidateCount > 1
                          orderby candidateCount, c.Id
                          select c.Id;

            var solutions = from cellId in cellIds
                            let itemsForThisCell = from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                                                   where proxy.SudokuBoard.Cells[cellId].Candidates[candidateValue]
                                                   select new TableingItem { CellId = cellId, CandidateValue = candidateValue }
                            let solution = TryTableing_Internal(proxy, itemsForThisCell.ToArray())
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        private SolveStep TryTableingByGroup(ISudokuBoardProxy proxy)
        {
            var solutions = from candidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                            from @group in proxy.SudokuBoard.Groups
                            let itemsForThisGroup = (from cellId in @group.CellIds
                                                     let cell = proxy.SudokuBoard.Cells[cellId]
                                                     where cell.Value == Candidate.NotSet && cell.Candidates[candidateValue]
                                                     select new TableingItem { CellId = cellId, CandidateValue = candidateValue }
                                                    ).ToArray()
                            where itemsForThisGroup.Length > 1
                            let solution = TryTableing_Internal(proxy, itemsForThisGroup)
                            where solution != null
                            select solution;

            return solutions.FirstOrDefault();
        }

        const string SolvingTechniqueName = "Table";

        private SolveStep TryTableing_Internal(ISudokuBoardProxy proxy, TableingItem[] items)
        {
            // Steps:
            // 1. For every cell/value combination start a fill and save the result in a separate var
            // 2. Try to use logic using the results to deduct something
            // Deductions...
            // (deductions from all results combined)
            // - a plus in a cell/candidate in all results -> always that plus, so always true
            // - a minus in a cell/candidate in all results -> always that minus, so always false
            // (deductions from one result)
            // - a plus and minus in a cell+candidate in one result -> contradiction, so the starting cell/value combo is invalid
            // - two or more plusses in a group in one result -> contradiction, so the starting cell/value combo is invalid
            // - no more candidates for a cell in one result -> so the starting cell/value combo is invalid
            // - no more cells for a candidate in a group in one result -> so the starting cell/value combo is invalid

            var addPlus = new BitSet[proxy.SudokuBoard.CandidateCount, items.Length];
            var addMinus = new BitSet[proxy.SudokuBoard.CandidateCount, items.Length];

            var tempPlus = new BitSet[proxy.SudokuBoard.CandidateCount];
            var tempMinus = new BitSet[proxy.SudokuBoard.CandidateCount];

            var solveStepItems = new List<SolveStepItem>();

            //step.Description = "Found a Tree from ";
            //for (var nr = 0; nr < items.Length; nr++)
            //{
            //    if (nr > 0) step.Description += "/";
            //    step.Description += proxy.SudokuBoard.Cells[items[nr].CellId].Name + "V" + Candidate.PrintValue(items[nr].CandidateValue);
            //}
            //step.Description += "\r\n";
            //if (showBigMsg)
            //    step.Description += Visualize();

            // First phase
            for (var p = 0; p < items.Length; p++)
            {
                // Clear temp arrays
                for (var i = 0; i < proxy.SudokuBoard.CandidateCount; i++)
                {
                    tempPlus[i] = new BitSet(proxy.SudokuBoard.CellCount, false);
                    tempMinus[i] = new BitSet(proxy.SudokuBoard.CellCount, false);
                }

                // Start recursive deduction
                //step.Description += "Trying candidate " + Candidate.printValue(value[p]) + " in cell " + this._cellList[cell[p]].Name + "\r\n";
                //var deductionResult = 
                RegisterCandidateConfirmation(proxy, tempPlus, tempMinus, items[p].CellId, items[p].CandidateValue);

                // Copy tempPlus and tempMinus to addPlus and addMinus
                for (var i = 0; i < proxy.SudokuBoard.CandidateCount; i++)
                {
                    addPlus[i, p] = tempPlus[i];
                    //addPlus[i, p] = deductionResult.Item1[i];
                    addMinus[i, p] = tempMinus[i];
                    //addMinus[i, p] = deductionResult.Item2[i];
                }
            }

            // step.Description += "Results\r\n";
            // Second phase
            // Check 1: a plus in all results, and a minus in all results
            for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
            {
                tempPlus[v] = new BitSet(proxy.SudokuBoard.CellCount, true);
                tempMinus[v] = new BitSet(proxy.SudokuBoard.CellCount, true);
            }
            for (var p = 0; p < items.Length; p++)
            {
                for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
                {
                    tempPlus[v] &= addPlus[v, p];
                    tempMinus[v] &= addMinus[v, p];
                }
            }
            // Check 1a: Check plusses
            for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
            {
                var eternalPlusCellIds = (from c in Enumerable.Range(0, proxy.SudokuBoard.CellCount)
                                          where tempPlus[v][c]
                                          where proxy.SudokuBoard.Cells[c].Value == Candidate.NotSet
                                          select c
                                         ).ToArray();

                //var cellIds = new List<int>();
                //for (var c = 0; c < proxy.SudokuBoard.CellCount; c++)
                //{
                //    if (tempPlus[v][c] && proxy.SudokuBoard.Cells[c].Value == -1)
                //    {
                //        cellIds.Add(c);
                //        //SetCell(c, v);
                //        //step.StepTaken = true;
                //        //step.Description += "- added a " + Candidate.PrintValue(v) + " in cell " + _cellList[c].Name + " (eternal plus)\r\n";
                //    }
                //}

                if (eternalPlusCellIds.Length > 0)
                {
                    var item = new SolveStepItem
                    {
                        SolveStepType = SolveStepItemType.CandidateConfirmation,
                        TechniqueName = SolvingTechniqueName,
                        CellIds = eternalPlusCellIds,
                        Value = v,
                        Explanation = "TODO"
                    };
                    solveStepItems.Add(item);
                }

                var eternalMinusCellIds = (from c in Enumerable.Range(0, proxy.SudokuBoard.CellCount)
                                           where tempMinus[v][c]
                                           where proxy.SudokuBoard.Cells[c].Candidates[v]
                                           select c
                                          ).ToArray();

                if (eternalMinusCellIds.Length > 0)
                {
                    var item = new SolveStepItem
                    {
                        SolveStepType = SolveStepItemType.CandidateRemoval,
                        TechniqueName = SolvingTechniqueName,
                        CellIds = eternalMinusCellIds,
                        Value = v,
                        Explanation = "TODO"
                    };
                    solveStepItems.Add(item);
                }
            }

            //// Check 1b: Check Minusses
            //for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
            //{
            //    for (var c = 0; c < proxy.SudokuBoard.CellCount; c++)
            //    {
            //        if (tempMinus[v][c] && proxy.SudokuBoard.Cells[c].Candidates[v])
            //        {
            //            _cellList[c].Candidates[v] = false;
            //            step.StepTaken = true;
            //            step.Description += "- removed candidate " + Candidate.PrintValue(v) + " from cell " + _cellList[c].Name + " (eternal minus)\r\n";
            //        }
            //    }
            //}

            /*
            // Check 2: deductions from one particular result
            var change = false;
            var count = 0;
            for (var p = 0; p < items.Length; p++)
            {
                for (var v = 0; v < proxy.SudokuBoard.CandidateCount; v++)
                {
                    change = false;
                    if (!change)
                    {
                        for (var c = 0; c < proxy.SudokuBoard.CellCount && !change; c++)
                        {
                            // Check 2a: A Plus and Minus in the same cell/candidate.
                            if (addPlus[v, p][c] && addMinus[v, p][c] && proxy.SudokuBoard.Cells[items[p].CellId].Candidates[items[p].CandidateValue])
                            {
                                _cellList[cell[p]].Candidates[value[p]] = false;
                                change = true;
                                step.StepTaken = true;
                                step.Description += "- removed candidate " + Candidate.PrintValue(value[p]) + " from cell " + _cellList[cell[p]].Name + " (single plus/minus combo with candidate " + Candidate.PrintValue(v) + " in cell " + _cellList[c].Name + ")\r\n";
                            }
                        }
                    }
                    if (!change)
                    {

                        for (int g = 0; g < proxy.SudokuBoard.Groups.Length && !change; g++)
                        {
                            // Check 2b: Two more plusses in a group
                            if (!change)
                            {
                                count = 0;
                                foreach (int gc in proxy.SudokuBoard.Groups[g].CellIds)
                                {
                                    if (addPlus[v, p][gc]) count++;
                                }
                                if (count >= 2 && proxy.SudokuBoard.Cells[items[p].CellId].Candidates[items[p].CandidateValue])
                                {
                                    proxy.SudokuBoard.Cells[items[p].CellId].Candidates[items[p].CandidateValue] = false;
                                    change = true;
                                    step.StepTaken = true;
                                    step.Description += "- removed candidate " + Candidate.PrintValue(value[p]) + " from cell " + _cellList[cell[p]].Name + " (single plus/plus combo in group " + _groupList[g].Name + ")\r\n";
                                }
                            }
                            // Check 2d: No more cells in this group for a candidate
                            if (!change)
                            {
                                if (!proxy.GroupHasNumber(g, v))
                                {
                                    count = 0;
                                    foreach (int gc in proxy.SudokuBoard.Groups[g].CellIds)
                                    {
                                        if (proxy.SudokuBoard.Cells[gc].Candidates[v] && !addMinus[v, p][gc])
                                        {
                                            count++;
                                        }
                                    }
                                    if (count == 0 && proxy.SudokuBoard.Cells[items[p].CellId].Candidates[items[p].CandidateValue])
                                    {
                                        proxy.SudokuBoard.Cells[items[p].CellId].Candidates[items[p].CandidateValue] = false;
                                        change = true;
                                        step.StepTaken = true;
                                        step.Description += "- removed candidate " + Candidate.PrintValue(value[p]) + " from cell " + _cellList[cell[p]].Name + " (elimination of candidate " + Candidate.PrintValue(v) + " in group " + _groupList[g].Name + ")\r\n";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */

            //if (!step.StepTaken)
            //    step.Description = string.Empty;
            //else
            //    if (showBigMsg)
            //    step.Description += Visualize();

            //return step;

            if (solveStepItems.Any())
            {
                return new SolveStep { Items = solveStepItems.ToArray() };
            }
            return null;
        }

        private static Tuple<BitSet[], BitSet[]> RegisterCandidateConfirmation(ISudokuBoardProxy proxy, BitSet[] plusBitSet, BitSet[] minusBitSet, int cellId, int candidateValue)
        {
            // Register the addition of a candidate confirmation
            plusBitSet[candidateValue][cellId] = true;

            // Register all related candidate removals in all related groups
            var candidatesToRemoveFromGroup = from @group in proxy.FindGroupsForCell(cellId)
                                              from cellToCheckId in @group.CellIds
                                              where cellToCheckId != cellId
                                              where !minusBitSet[candidateValue][cellToCheckId]
                                              where proxy.SudokuBoard.Cells[cellToCheckId].Candidates[candidateValue]
                                              select cellToCheckId;
            foreach (var candidateToRemoveFromGroup in candidatesToRemoveFromGroup)
            {
                RegisterCandidateRemoval(proxy, plusBitSet, minusBitSet, candidateToRemoveFromGroup, candidateValue);
            }

            // Register all related candidate removals in this cell
            var candidatesToRemove = from otherCandidate in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                                     where otherCandidate != candidateValue
                                     where !minusBitSet[otherCandidate][cellId]
                                     where proxy.SudokuBoard.Cells[cellId].Candidates[otherCandidate]
                                     select otherCandidate;
            foreach (var candidateToRemove in candidatesToRemove)
            {
                RegisterCandidateRemoval(proxy, plusBitSet, minusBitSet, cellId, candidateToRemove);
            }

            return Tuple.Create(plusBitSet, minusBitSet);
        }

        private static void RegisterCandidateRemoval(ISudokuBoardProxy proxy, BitSet[] plusBitSet, BitSet[] minusBitSet, int cellId, int candidateValue)
        {
            // Register the addition of a candidate removal
            minusBitSet[candidateValue][cellId] = true;

            // Try to deduce a candidate confirmation in all related groups since we added a candidate removal
            var deducedPlusCellIds = from @group in proxy.FindGroupsForCell(cellId)
                                     let notMinusCells = (from cellToCheckId in @group.CellIds
                                                          where cellToCheckId != cellId
                                                          where !minusBitSet[candidateValue][cellToCheckId]
                                                          select cellToCheckId
                                                         ).ToArray()
                                     where notMinusCells.Length == 1
                                     where !plusBitSet[candidateValue][notMinusCells.First()]
                                     select notMinusCells.First();

            foreach (var deducedPlusCellId in deducedPlusCellIds)
            {
                RegisterCandidateConfirmation(proxy, plusBitSet, minusBitSet, deducedPlusCellId, candidateValue);
            }

            // Try to deduce a candidate confirmation in this cell since we added a candidate removal
            var leftOverCandidates = (from otherCandidateValue in Enumerable.Range(0, proxy.SudokuBoard.CandidateCount)
                                      where otherCandidateValue != candidateValue
                                      where !minusBitSet[otherCandidateValue][cellId]
                                      where proxy.SudokuBoard.Cells[cellId].Candidates[otherCandidateValue]
                                      select otherCandidateValue
                                     ).ToArray();
            if (leftOverCandidates.Length == 1)
            {
                RegisterCandidateConfirmation(proxy, plusBitSet, minusBitSet, cellId, leftOverCandidates.First());
            }
        }
    }
}
