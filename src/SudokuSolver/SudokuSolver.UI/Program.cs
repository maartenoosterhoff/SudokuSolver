using System;
using System.Diagnostics;
using SudokuSolver.Core.Builders;
using SudokuSolver.Core.Engine;
using SudokuSolver.Core.Models;
using SudokuSolver.Core.Parsers;
using SudokuSolver.Core.Solvers;
using SudokuSolver.Core.Solvers.Techniques;
using SudokuSolver.Core.Visualizers;

namespace SudokuSolver.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            ISudokuBoardBuilder builder = new SudokuBoardBuilderProxy(
                new[]
                {
                    new Classis9x9SudokuBoardBuilder()
                }
            );

            SudokuBoard board = builder.Build(SudokuType.Classic9by9);
            ISudokuBoardProxy proxy = new SudokuBoardProxy(board);
            ISudokuParser parser = new SudokuParser();
            // solvable with: locked-candidates/naked-multiple
            //parser.ParseInto(proxy, "_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_");
            // solvable with: naked-multiple/locked-candidates/hidden-single
            //parser.ParseInto(proxy, "000074316000603840000008500725800034000030050000002798008940000040085900971326485");
            // solvable with:  naked-single/naked-multiple/hidden-single
            //parser.ParseInto(proxy, "_____4_718__21______7_9_3________4262_______7659________5_6_1______49__541_3_____"); // OK
            // solvable with: naked-single/hidden-single/locked-candidates/naked-multiple
            //parser.ParseInto(proxy, "..9.4.8.1.376.....6.............5.....8.1.7.....4.............7.....732.5.4.2.9..");

            // unsolvable with: hidden-single/locked-candidates/naked-multiple
            //parser.ParseInto(proxy, "708000300000201000500000000040000026300080000000100090090600004000070500000000000");
            // unsolvable with: hidden-single/locked-candidates
            //parser.ParseInto(proxy, ".....725..1.58...3..4..9.....7...59.3...5.1..2....6.....63..8.5.......7.1..7.....");


            // ??
            //parser.ParseInto(proxy, "049132000081479000327685914096051800075028000038046005853267000712894563964513000");

            // ??
            parser.ParseInto(proxy, "..9.4.8.1.376.....6.............5.....8.1.7.....4.............7.....732.5.4.2.9..");

            ISudokuVisualizer visualizer = new SimpleSudokuVisualizer(proxy);
            Console.WriteLine(visualizer.Visualize());

            var engine = new SimpleSudokuSolverEngine(
                new ISolvingTechnique[]
                {
                    new NakedSingleSolvingTechnique(),
                    new HiddenSingleSolvingTechnique(),
                    new LockedCandidateSolvingTechnique(),
                    new NakedMultipleSolvingTechnique(),
                    new HiddenMultipleSolvingTechnique(),
                    new AlignedPairExclusionSolvingTechnique()
                }
            );
            var sw = Stopwatch.StartNew();
            engine.Solve(proxy);
            sw.Stop();

            Console.WriteLine("Solve time: {0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine(visualizer.Visualize());
            Console.ReadLine();
        }
    }
}
