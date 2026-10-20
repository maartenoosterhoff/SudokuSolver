using System.Diagnostics;
using SudokuSolver.NET.Builders;
using SudokuSolver.NET.Engine;
using SudokuSolver.NET.Engine.Techniques;
using SudokuSolver.NET.Models;
using SudokuSolver.NET.Printing;

namespace SudokuSolver.NET;

internal class Program
{
    static void Main(string[] args)
    {
        const string Sudoku9x9_NAKED_TRIPLE = "_____4_718__21______7_9_3________4262_______7659________5_6_1______49__541_3_____";
        const string Sudoku9x9_HIDDEN_DOUBLE = "_________9_46_7____768_41__3_97_1_8_7_8___3_1_513_87_2__75_261___54_32_8_________";
        const string Sudoku9X9_HIDDEN_QUAD = "9678324515__49786_8__165_79_925__78___________3___951_3_69_8__727_3_6__8_8972_635";
        const string Sudoku9x9_LOCKED_CANDIDATE = "_2_____7_9__5_8__4_________4___3___8_7__9__2_6___1___5_________5__6_4__1_3_____9_";
        const string Sudoku9x9_LOCKED_CANDIDATE_2 = "..9.4.8.1.376.....6.............5.....8.1.7.....4.............7.....732.5.4.2.9..";
        const string Sudoku9x9_ALIGNED_PAIR_EXCLUSION = "..9.4.8.1.376.....6.............5.....8.1.7.....4.............7.....732.5.4.2.9..";

        var sudoku = SudokuBuilder.SomeSudoku
            .WithType(SudokuType.Classic9By9)
            .WithContents(Sudoku9x9_ALIGNED_PAIR_EXCLUSION)
            .Build();

        // HIDDEN_QUAD NOT DETECTED.
        Console.WriteLine(sudoku.Print());
        var sw = Stopwatch.StartNew();
        sudoku.Solve(
            o =>
            {
                o.DisplayBetweenSteps = false;
                //o.Techniques
                    //.Except(Technique.NakedSingle)
                    //.Except(Technique.HiddenSingle)
                    //.Except(Technique.NakedMultiple)
                    //.Except(Technique.HiddenMultiple)
                    //.Except(Technique.LockedCandidate)
                    //.Except(Technique.AlignedPairExclusion)
                    ;
            });
        //sudoku.Solve();
        //sudoku.Cells.Single(x => x.Name == "R4C1").RemoveCandidate(0);
        //sudoku.Cells.Single(x => x.Name == "R5C6").RemoveCandidate(0);
        //sudoku.Cells.Single(x => x.Name == "R5C8").RemoveCandidate(3);
        //Console.WriteLine(sudoku.Print());
        //sudoku.Solve();
        sw.Stop();
        Console.WriteLine("Solve time: {0}ms", sw.ElapsedMilliseconds);
        Console.WriteLine(sudoku.Print());

        Console.ReadKey();
    }
}