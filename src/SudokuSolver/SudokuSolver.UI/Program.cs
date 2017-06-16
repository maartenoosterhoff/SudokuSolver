using System;
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
            // naked single + multiples
            //parser.ParseInto(proxy, "_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_");
            // hidden single
            parser.ParseInto(proxy, "000074316000603840000008500725800034000030050000002798008940000040085900971326485");

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

            engine.Solve(proxy);
            Console.WriteLine(visualizer.Visualize());
            Console.ReadLine();
        }
    }
}
