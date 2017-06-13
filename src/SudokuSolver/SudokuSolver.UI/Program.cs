using System;
using SudokuSolver.Core;

namespace SudokuSolver.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            SudokuSolverEngine e = new SudokuSolverEngine(SudokuType.CLASSIC9BY9);
            e.ParseSudokuString("_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_");
            e.Visualize();
            e.Solve();
            e.Visualize();
            Console.Write(e.SolutionText);
            Console.ReadLine();
        }
    }
}
