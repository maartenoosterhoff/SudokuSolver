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
            parser.ParseInto(proxy, "_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_");
            // solvable with: naked-multiple/locked-candidate/hidden-single
            //parser.ParseInto(proxy, "000074316000603840000008500725800034000030050000002798008940000040085900971326485");
            // solvable with:  naked-single/naked-multiple/hidden-single
            //parser.ParseInto(proxy, "_____4_718__21______7_9_3________4262_______7659________5_6_1______49__541_3_____"); // OK

            ISudokuVisualizer visualizer = new SimpleSudokuVisualizer(proxy);
            Console.WriteLine(visualizer.Visualize());

            var engine = new SimpleSudokuSolverEngine(
                new ISolvingTechnique[]
                {
                    new NakedSingleSolvingTechnique(),
                    new HiddenSingleSolvingTechnique(),
                    new LockedCandidateSolvingTechnique(),
                    new NakedMultipleSolvingTechnique(),
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
