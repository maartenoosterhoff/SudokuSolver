using System.Linq;
using System.Collections.Generic;

namespace SudokuSolver.Core.Models
{
    partial class Solution
    {
        /// <summary>
        /// Tries to solve the sudoku by finding a Tree, and several possible deductions because of it.
        /// </summary>
        /// <returns>The description of the step in the solution</returns>
        private SolutionStep TryTableing()
        {
            var step = new SolutionStep(false);

            if (!step.StepTaken) step = TryTableingByCell();
            if (!step.StepTaken) step = TryTableingByGroup();

            return step;
        }

        private SolutionStep TryTableingByCell()
        {
            var step = new SolutionStep(false);

            var sortedCells = from c in _cellList
                                 let candidateCount = c.CandidateCount()
                                 where candidateCount > 1
                                 orderby candidateCount, c.ID                                 
                                 select new CellSorter(c.ID, candidateCount);

            var cellSorterList = sortedCells.ToList();

            //var cellSorterList = new List<CellSorter>();
            //foreach (Cell c in _cellList) {
            //    if (c.CandidateCount() > 1) {
            //        cellSorterList.Add(new CellSorter(c.ID, c.CandidateCount()));
            //    }
            //}
            //cellSorterList.Sort(); // Standard sorting in this class is sorted by candidatecount, then cellid


            for (var c = 0; c < cellSorterList.Count && !step.StepTaken; c++) {
                var cell = new List<int>();
                var value = new List<int>();
                var cellId = cellSorterList[c].CellId;
                for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                    if (_cellList[cellId].Candidates[v]) {
                        cell.Add(cellId);
                        value.Add(v);
                    }
                }
                step = TryTableingExecute(cell, value);
            }

            return step;
        }

        private SolutionStep TryTableingByGroup()
        {
            var step = new SolutionStep(false);

            for (var v = 0; v < Candidate.PossibleCandidateCount && !step.StepTaken; v++) {
                for (var g = 0; g < _groupList.Count && !step.StepTaken; g++) {
                    var cell = new List<int>();
                    var value = new List<int>();
                    for (var gc = 0; gc < _groupList[g].Cells.Count; gc++) {
                        var c = _cellList[_groupList[g].Cells[gc]];
                        if (c.Value == -1 && c.Candidates[v]) {
                            cell.Add(c.ID);
                            value.Add(v);
                        }
                    }
                    if (cell.Count > 1 || value.Count > 1) {
                        step = TryTableingExecute(cell, value);
                    }
                }
            }
            return step;
        }

        private SolutionStep TryTableingExecute(List<int> cell, List<int> value)
        {
            var showBigMsg = false;

            if (cell.Count != value.Count) {
                throw new System.Exception("The dimension of the cell-list must be equal to the dimension of the value-list");
            }

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

            var step = new SolutionStep(false);
            var addPlus = new BitLayer[Candidate.PossibleCandidateCount, cell.Count];
            var addMinus = new BitLayer[Candidate.PossibleCandidateCount, cell.Count];

            var tempPlus = new BitLayer[Candidate.PossibleCandidateCount];
            var tempMinus = new BitLayer[Candidate.PossibleCandidateCount];

            step.Description = "Found a Tree from ";
            for (var nr = 0; nr < cell.Count; nr++) {
                if (nr > 0) step.Description += "/";
                step.Description += _cellList[cell[nr]].Name + "V" + Candidate.PrintValue(value[nr]);
            }
            step.Description += "\r\n";
            if (showBigMsg)
                step.Description += Visualize();

            // First phase
            for (var p = 0; p < cell.Count; p++) {
                // Clear temp arrays
                for (var i = 0; i < Candidate.PossibleCandidateCount; i++) {
                    tempPlus[i] = new BitLayer(Dimension, false);
                    tempMinus[i] = new BitLayer(Dimension, false);
                }

                // Start recursive deduction
                //step.Description += "Trying candidate " + Candidate.printValue(value[p]) + " in cell " + this._cellList[cell[p]].Name + "\r\n";
                TryTableingAddPlus(ref tempPlus, ref tempMinus, cell[p], value[p], step);

                // Copy tempPlus and tempMinus to addPlus and addMinus
                for (var i = 0; i < Candidate.PossibleCandidateCount; i++) {
                    addPlus[i, p] = tempPlus[i];
                    addMinus[i, p] = tempMinus[i];
                }
            }

            // step.Description += "Results\r\n";
            // Second phase
            // Check 1: a plus in all results, and a minus in all results
            for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                tempPlus[v] = new BitLayer(Dimension, true);
                tempMinus[v] = new BitLayer(Dimension, true);
            }
            for (var p = 0; p < cell.Count; p++) {
                for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                    tempPlus[v] &= addPlus[v, p];
                    tempMinus[v] &= addMinus[v, p];
                }
            }
            // Check 1a: Check plusses
            for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                for (var c = 0; c < Dimension; c++) {
                    if (tempPlus[v].Layer[c] && _cellList[c].Value == -1) {
                        SetCell(c, v);
                        step.StepTaken = true;
                        step.Description += "- added a " + Candidate.PrintValue(v) + " in cell " + _cellList[c].Name + " (eternal plus)\r\n";
                    }
                }
            }
            // Check 1b: Check Minusses
            for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                for (var c = 0; c < Dimension; c++) {
                    if (tempMinus[v].Layer[c] && _cellList[c].Candidates[v]) {
                        _cellList[c].Candidates[v] = false;
                        step.StepTaken = true;
                        step.Description += "- removed candidate " + Candidate.PrintValue(v) + " from cell " + _cellList[c].Name + " (eternal minus)\r\n";
                    }
                }
            }
            // Check 2: deductions from one particular result
            var change = false;
            var count = 0;
            for (var p = 0; p < cell.Count; p++) {
                for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                    change = false;
                    if (!change) {
                        for (var c = 0; c < Dimension && !change; c++) {
                            // Check 2a: A Plus and Minus in the same cell/candidate.
                            if (addPlus[v, p].Layer[c] && addMinus[v, p].Layer[c] &&
                                    _cellList[cell[p]].Candidates[value[p]]) {
                                _cellList[cell[p]].Candidates[value[p]] = false;
                                change = true;
                                step.StepTaken = true;
                                step.Description += "- removed candidate " + Candidate.PrintValue(value[p]) + " from cell " + _cellList[cell[p]].Name + " (single plus/minus combo with candidate " + Candidate.PrintValue(v) + " in cell " + _cellList[c].Name + ")\r\n";
                            }
                        }
                    }
                    if (!change) {

                        for (int g = 0; g < _groupList.Count && !change; g++) {
                            // Check 2b: Two more plusses in a group
                            if (!change) {
                                count = 0;
                                foreach (int gc in _groupList[g].Cells)
                                    if (addPlus[v, p].Layer[gc]) count++;
                                if (count >= 2 && _cellList[cell[p]].Candidates[value[p]]) {
                                    _cellList[cell[p]].Candidates[value[p]] = false;
                                    change = true;
                                    step.StepTaken = true;
                                    step.Description += "- removed candidate " + Candidate.PrintValue(value[p]) + " from cell " + _cellList[cell[p]].Name + " (single plus/plus combo in group " + _groupList[g].Name + ")\r\n";
                                }
                            }
                            // Check 2d: No more cells in this group for a candidate
                            if (!change) {
                                if (!GroupHasNumber(g, v)) {
                                    count = 0;
                                    foreach (int gc in _groupList[g].Cells)
                                        if (_cellList[gc].Candidates[v] && !addMinus[v, p].Layer[gc])
                                            count++;
                                    if (count == 0 && _cellList[cell[p]].Candidates[value[p]]) {
                                        _cellList[cell[p]].Candidates[value[p]] = false;
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

            if (!step.StepTaken)
                step.Description = string.Empty;
            else
                if (showBigMsg)
                    step.Description += Visualize();

            return step;
        }

        private void TryTableingAddPlus(ref BitLayer[] tempPlus, ref BitLayer[] tempMinus, int cellId, int value, SolutionStep step)
        {
            //step.Description += "- added a plus in candidate " + Candidate.printValue(value) + " in cell " + this._cellList[cellId].Name + "\r\n";
            tempPlus[value].Layer[cellId] = true;
            var groups = FindGroupsForCell(cellId);
            foreach (var g in groups) {
                foreach (var nextCell in g.Cells) {
                    if (nextCell != cellId && !tempMinus[value].Layer[nextCell] && _cellList[nextCell].Candidates[value]) {
                        TryTableingAddMinus(ref tempPlus, ref tempMinus, nextCell, value, step);
                    }
                }
            }
            for (int v = 0; v < Candidate.PossibleCandidateCount; v++) {
                if (v != value && !tempMinus[v].Layer[cellId] && _cellList[cellId].Candidates[v]) {
                    TryTableingAddMinus(ref tempPlus, ref tempMinus, cellId, v, step);
                }
            }
        }

        private void TryTableingAddMinus(ref BitLayer[] tempPlus, ref BitLayer[] tempMinus, int cellId, int value, SolutionStep step)
        {
            //step.Description += "- added a minus in candidate " + Candidate.printValue(value) + " in cell " + this._cellList[cellId].Name + "\r\n";
            tempMinus[value].Layer[cellId] = true;
            var groups = FindGroupsForCell(cellId);
            var count = 0;
            var lastCellId = -1;
            var lastValue = -1;

            // Try all cells in all related groups
            foreach (var g in groups) {
                foreach (var nextCell in g.Cells) {
                    if (!tempMinus[value].Layer[nextCell]) {
                        count++;
                        lastCellId = nextCell;
                    }
                }
                if (count == 1) {
                    if (!tempPlus[value].Layer[lastCellId]) {
                        TryTableingAddPlus(ref tempPlus, ref tempMinus, lastCellId, value, step);
                    }
                }
            }

            // Try this cell
            count = 0;
            lastValue = -1;
            for (var v = 0; v < Candidate.PossibleCandidateCount; v++) {
                if (!tempMinus[v].Layer[cellId] && _cellList[cellId].Candidates[v]) {
                    count++;
                    lastValue = v;
                }
            }
            if (count == 1) {
                TryTableingAddPlus(ref tempPlus, ref tempMinus, cellId, lastValue, step);
            }
        }
    }
}
