using System;
using SudokuSolver.Core.Builders;
using SudokuSolver.Core.Models;
using SudokuSolver.Core.Parsers;
using SudokuSolver.Core.Visualizers;

namespace SudokuSolver.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            ISudokuBoardBuilder builder = new SudokuBoardBuilderProxy(
                new []
                {
                    new Classis9x9SudokuBoardBuilder()
                }
            );
            SudokuBoard board = builder.Build(SudokuType.Classic9by9);
            ISudokuBoardProxy proxy = new SudokuBoardProxy(board);
            ISudokuParser parser = new SudokuParser();
            parser.ParseInto(proxy, "_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_");
            ISudokuVisualizer visualizer = new SimpleSudokuVisualizer(proxy);
            Console.WriteLine(visualizer.Visualize());





            //SudokuSolverEngine e = new SudokuSolverEngine(SudokuType.Classic9by9);
            //e.ParseSudokuString("_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_");
            //e.Visualize();
            //e.Solve();
            //e.Visualize();
            //Console.Write(e.SolutionText);
            Console.ReadLine();
        }
    }
}
