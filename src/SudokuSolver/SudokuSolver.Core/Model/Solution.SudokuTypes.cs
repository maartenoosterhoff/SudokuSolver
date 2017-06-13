using System;
using System.Collections.Generic;
using System.Text;

namespace SudokuSolver.Core
{
    /// <summary>
    /// The different types of sudoku.
    /// </summary>
    public enum SudokuType
    {
        CLASSIC9BY9,
        CLASSICPLUS4BLOCKS,
        SUDOKU16BY16,
        XSUDOKU
    }

    partial class Solution
    {
        /// <summary>
        /// Create a sudoku.
        /// </summary>
        private void CreateSudoku()
        {
            switch (_sudokuType) {

                // The classic 9x9 sudoku
                case SudokuType.CLASSIC9BY9:
                // The classic 9x9 sudoku plus 4 3x3 blocks
                case SudokuType.CLASSICPLUS4BLOCKS:
                // The 9x9 Sudoku with the diagonals included
                case SudokuType.XSUDOKU:

                    Candidate.PossibleCandidateCount = 9;
                    Dimension = 81;
                    _cellList = new Cell[Dimension];
                    for (int i = 0; i < Dimension; i++) {
                        // i = r*9+c
                        // col = i % 9
                        // row = (i-col)/9 = (i-i%9)/9
                        _cellList[i] = new Cell(i, "R" + (((i - (i % 9)) / 9) + 1).ToString() + "C" + ((i % 9) + 1).ToString());
                    }

                    break;
                case SudokuType.SUDOKU16BY16:
                    Candidate.PossibleCandidateCount = 16;
                    Dimension = 16 * 16;
                    _cellList = new Cell[Dimension];
                    for (int i = 0; i < Dimension; i++) {
                        // i = r*16+c
                        // col = i % 16
                        // row = (i-col)/16 = (i-i%16)/16
                        _cellList[i] = new Cell(i, "R" + (((i - (i % 16)) / 16) + 1).ToString() + "C" + ((i % 16) + 1).ToString());
                    }
                    break;
                default:
                    throw new System.Exception("Unknown Sudokutype!");
                //break;
            }
            AddGroups();
            PopulateGroupOverlapData();
        }

        /// <summary>
        /// Create the different groups for the different kind of sudoku's.
        /// </summary>
        public void AddGroups()
        {
            Group g;
            int dim = -1, dimBlock = -1;
            int gId = 0;

            _groupList = new List<Group>();

            switch (_sudokuType) {
                case SudokuType.CLASSIC9BY9:
                case SudokuType.CLASSICPLUS4BLOCKS:
                case SudokuType.XSUDOKU:
                    dim = 9;
                    dimBlock = 3;

                    break;
                case SudokuType.SUDOKU16BY16:
                    dim = 16;
                    dimBlock = 4;
                    break;

                default:

                    break;
            }

            if (_sudokuType == SudokuType.CLASSIC9BY9 || _sudokuType == SudokuType.CLASSICPLUS4BLOCKS ||
                _sudokuType == SudokuType.SUDOKU16BY16 || _sudokuType == SudokuType.XSUDOKU) {

                // Rows
                for (int r = 0; r < dim; r++) {
                    g = new Group(gId++, "R" + (r + 1).ToString());
                    for (int c = 0; c < dim; c++) {
                        g.AddCell(_cellList[(r * dim) + c].ID);
                    }
                    _groupList.Add(g);
                }

                // Columns
                for (int c = 0; c < dim; c++) {
                    g = new Group(gId++, "C" + (c + 1).ToString());
                    for (int r = 0; r < dim; r++) {
                        g.AddCell(_cellList[(r * dim) + c].ID);
                    }
                    _groupList.Add(g);
                }

                // Blocks
                for (int r0 = 0; r0 < dim; r0 += dimBlock) {
                    for (int c0 = 0; c0 < dim; c0 += dimBlock) {
                        g = new Group(gId++, "B" + (r0 + 1).ToString() + (c0 + 1).ToString());
                        for (int r = r0; r < r0 + dimBlock; r++) {
                            for (int c = c0; c < c0 + dimBlock; c++) {
                                g.AddCell(_cellList[(r * dim) + c].ID);
                            }
                        }
                        _groupList.Add(g);
                    }
                }
            }
            if (_sudokuType == SudokuType.CLASSICPLUS4BLOCKS) {
                g = new Group(gId++, "B22");
                for (int r = 1; r < 4; r++)
                    for (int c = 1; c < 4; c++)
                        g.AddCell(_cellList[(r * dim) + c].ID);
                _groupList.Add(g);
                g = new Group(gId++, "B62");
                for (int r = 5; r < 8; r++)
                    for (int c = 1; c < 4; c++)
                        g.AddCell(_cellList[(r * dim) + c].ID);
                _groupList.Add(g);
                g = new Group(gId++, "B26");
                for (int r = 1; r < 4; r++)
                    for (int c = 5; c < 8; c++)
                        g.AddCell(_cellList[(r * dim) + c].ID);
                _groupList.Add(g);
                g = new Group(gId++, "B66");
                for (int r = 5; r < 8; r++)
                    for (int c = 5; c < 8; c++)
                        g.AddCell(_cellList[(r * dim) + c].ID);
                _groupList.Add(g);
            }
            if (_sudokuType == SudokuType.XSUDOKU) {
                g = new Group(gId++, "D1");
                for (int i = 0; i < dim; i++) {
                    g.AddCell(_cellList[(i * dim) + i].ID);
                }
                _groupList.Add(g);
                g = new Group(gId++, "D2");
                for (int i = 0; i < dim; i++) {
                    g.AddCell(_cellList[(i * dim) + (dim - i - 1)].ID);
                }
                _groupList.Add(g);
            }
        }

        /// <summary>
        /// Visualizes the sudoku in textformat.
        /// </summary>
        /// <returns>The sudoku in textformat</returns>
        public string Visualize()
        {
            string result = "\r\n";
            string bar;
            int max;
            string line;
            Cell cell;
            int candidateCount;

            switch (_sudokuType) {
                case SudokuType.CLASSIC9BY9:
                case SudokuType.CLASSICPLUS4BLOCKS:
                case SudokuType.XSUDOKU:
                    max = -1;
                    foreach (Cell c in _cellList)
                        if (c.CandidateCount() > max) max = c.CandidateCount();
                    if (max < 4)
                        max = 4;
                    if (max == 9)
                        max = 8;

                    bar = "";
                    for (int i = 0; i < (9 * max + 8); i++)
                        bar += "-";

                    for (int r = 0; r < 9; r++) {
                        line = string.Empty;
                        if (r == 3 || r == 6)
                            result += bar + "\r\n";
                        for (int c = 0; c < 9; c++) {
                            if (c == 3 || c == 6) {
                                line += "|";
                            }
                            else if (c != 0) {
                                line += " ";
                            }
                            cell = _cellList[(r * 9) + c];
                            if (cell.Value != -1) {
                                line += cell.ToString();
                                for (int i = 0; i < max - 1; i++)
                                    line += " ";
                            }
                            else {
                                candidateCount = 0;
                                if (cell.CandidateCount() == 9) {
                                    line += "all";
                                    for (int i = 0; i < max - 3; i++)
                                        line += " ";
                                }
                                else {
                                    for (int i = 0; i < Candidate.PossibleCandidateCount; i++) {
                                        if (cell.Candidates[i]) {
                                            candidateCount++;
                                            line += Candidate.PrintValue(i);
                                        }
                                    }
                                    for (int i = 0; i < (max - candidateCount); i++)
                                        line += " ";
                                }
                            }
                        }
                        result += line + "\r\n";
                    }
                    break;

                case SudokuType.SUDOKU16BY16:
                    max = -1;
                    foreach (Cell c in _cellList)
                        if (c.CandidateCount() > max) max = c.CandidateCount();
                    if (max < 5)
                        max = 5;
                    if (max == 16)
                        max = 15;

                    bar = "";
                    for (int i = 0; i < (16 * max + 15); i++)
                        bar += "-";

                    for (int r = 0; r < 16; r++) {
                        line = string.Empty;
                        if (r == 4 || r == 8 || r == 12)
                            result += bar + "\r\n";
                        for (int c = 0; c < 16; c++) {
                            if (c == 4 || c == 8 || c == 12) {
                                line += "|";
                            }
                            else if (c != 0) {
                                line += " ";
                            }
                            cell = _cellList[(r * 16) + c];
                            if (cell.Value != -1) {
                                line += cell.ToString();
                                for (int i = 0; i < max - 1; i++)
                                    line += " ";
                            }
                            else {
                                candidateCount = 0;
                                if (cell.CandidateCount() == 16) {
                                    line += "all";
                                    for (int i = 0; i < max - 3; i++)
                                        line += " ";
                                }
                                else {
                                    for (int i = 0; i < Candidate.PossibleCandidateCount; i++) {
                                        if (cell.Candidates[i]) {
                                            candidateCount++;
                                            line += Candidate.PrintValue(i);
                                        }
                                    }
                                    for (int i = 0; i < (max - candidateCount); i++)
                                        line += " ";
                                }
                            }
                        }
                        result += line + "\r\n";
                    }
                    break;
                default:
                    break;
            }

            return result + "\r\n";
        }

        /// <summary>
        /// Parses a sudokustring into memory.
        /// </summary>
        /// <param name="sudokuString">The sudoku in textformat</param>
        /// <returns>True if the parsing succeeded, False if not</returns>
        public bool parseSudoku(string sudokuString)
        {
            bool result = false;
            bool formatOk = false;

            if (sudokuString.Length == Dimension) {
                formatOk = true;
            }

            if (formatOk) {
                Reset();
                _sudokuState = SudokuState.RESET;
                char[] m = sudokuString.ToCharArray();
                int c;
                for (int i = 0; i < Dimension; i++) {
                    c = Candidate.Parse(m[i]);
                    if (c != -1)
                        SetCell(i, c);
                }
                result = true;
            }

            return result;
        }
    }
}
