using System;
using System.Linq;
using SudokuSolver.Core.Extensions;
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
            int max = _proxy.SudokuBoard.Cells.Max(x => x.CurrentCandidateCount());
            string line;
            Cell cell;
            int candidateCount;

            switch (_proxy.SudokuBoard.SudokuType)
            {
                case SudokuType.Classic9By9:
                case SudokuType.Classic9By9Plus4:
                case SudokuType.XSudoku:
                    if (max < 4)
                        max = 4;
                    if (max == 9)
                        max = 8;

                    bar = string.Empty.PadRight(9 * max + 8, '-');

                    for (int r = 0; r < 9; r++)
                    {
                        line = string.Empty;
                        if (r.In(3, 6))
                            result += bar + "\r\n";
                        for (int c = 0; c < 9; c++)
                        {
                            if (c.In(3, 6))
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
                                line += GetCandidatePrintValue(cell.Value);
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

                case SudokuType.Sudoku16By16:
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
                                line += GetCandidatePrintValue(cell.Value);
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
