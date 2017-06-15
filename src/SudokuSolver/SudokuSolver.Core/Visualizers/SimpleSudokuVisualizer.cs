using System;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Visualizers
{
    public interface ISudokuVisualizer
    {
        string GetCandidatePrintValue(int value);
        string Visualize();
    }

    public class SimpleSudokuVisualizer : ISudokuVisualizer
    {
        public SimpleSudokuVisualizer(ISudokuBoardProxy proxy)
        {
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            _proxy = proxy;
        }

        private readonly ISudokuBoardProxy _proxy;

        public string GetCandidatePrintValue(int value)
        {
            if (value <= 8)
                return (value + 1).ToString();
            return ((char)((value - 9) + 'A')).ToString();
        }

        public string Visualize()
        {
            string result = "\r\n";
            string bar;
            int max;
            string line;
            Cell cell;
            int candidateCount;

            switch (_proxy.SudokuBoard.SudokuType)
            {
                case SudokuType.Classic9by9:
                case SudokuType.Classic9by9Plus4:
                case SudokuType.XSudoku:
                    max = -1;
                    foreach (Cell c in _proxy.SudokuBoard.Cells)
                        if (c.CurrentCandidateCount() > max) max = c.CurrentCandidateCount();
                    if (max < 4)
                        max = 4;
                    if (max == 9)
                        max = 8;

                    bar = "";
                    for (int i = 0; i < (9 * max + 8); i++)
                        bar += "-";

                    for (int r = 0; r < 9; r++)
                    {
                        line = string.Empty;
                        if (r == 3 || r == 6)
                            result += bar + "\r\n";
                        for (int c = 0; c < 9; c++)
                        {
                            if (c == 3 || c == 6)
                            {
                                line += "|";
                            }
                            else if (c != 0)
                            {
                                line += " ";
                            }
                            cell = _proxy.SudokuBoard.Cells[(r * 9) + c];
                            if (cell.Value != -1)
                            {
                                line += cell.ToString();
                                for (int i = 0; i < max - 1; i++)
                                    line += " ";
                            }
                            else
                            {
                                candidateCount = 0;
                                if (cell.CurrentCandidateCount() == 9)
                                {
                                    line += "all";
                                    for (int i = 0; i < max - 3; i++)
                                        line += " ";
                                }
                                else
                                {
                                    for (int i = 0; i < _proxy.SudokuBoard.CandidateCount; i++)
                                    {
                                        if (cell.Candidates[i])
                                        {
                                            candidateCount++;
                                            line += GetCandidatePrintValue(i);
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

                case SudokuType.Sudoku16by16:
                    max = -1;
                    foreach (Cell c in _proxy.SudokuBoard.Cells)
                        if (c.CurrentCandidateCount() > max) max = c.CurrentCandidateCount();
                    if (max < 5)
                        max = 5;
                    if (max == 16)
                        max = 15;

                    bar = "";
                    for (int i = 0; i < (16 * max + 15); i++)
                        bar += "-";

                    for (int r = 0; r < 16; r++)
                    {
                        line = string.Empty;
                        if (r == 4 || r == 8 || r == 12)
                            result += bar + "\r\n";
                        for (int c = 0; c < 16; c++)
                        {
                            if (c == 4 || c == 8 || c == 12)
                            {
                                line += "|";
                            }
                            else if (c != 0)
                            {
                                line += " ";
                            }
                            cell = _proxy.SudokuBoard.Cells[(r * 16) + c];
                            if (cell.Value != -1)
                            {
                                line += cell.ToString();
                                for (int i = 0; i < max - 1; i++)
                                    line += " ";
                            }
                            else
                            {
                                candidateCount = 0;
                                if (cell.CurrentCandidateCount() == 16)
                                {
                                    line += "all";
                                    for (int i = 0; i < max - 3; i++)
                                        line += " ";
                                }
                                else
                                {
                                    for (int i = 0; i < _proxy.SudokuBoard.CandidateCount; i++)
                                    {
                                        if (cell.Candidates[i])
                                        {
                                            candidateCount++;
                                            line += GetCandidatePrintValue(i);
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
    }
}
