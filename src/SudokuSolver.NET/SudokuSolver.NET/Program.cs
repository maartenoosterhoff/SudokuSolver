using SudokuSolver.NET.Builders;
using SudokuSolver.NET.Engine;
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

        var sudoku = SudokuBuilder.SomeSudoku
            .WithType(SudokuType.Classic9By9)
            .WithContents(Sudoku9X9_HIDDEN_QUAD)
            .Build();

        // HIDDEN_QUAD NOT DETECTED.
        Console.WriteLine(sudoku.Print());
        sudoku.Solve();
        sudoku.Cells.Single(x => x.Name == "R4C1").RemoveCandidate(0);
        sudoku.Cells.Single(x => x.Name == "R5C6").RemoveCandidate(0);
        sudoku.Cells.Single(x => x.Name == "R5C8").RemoveCandidate(3);
        Console.WriteLine(sudoku.Print());
        sudoku.Solve();
        Console.WriteLine(sudoku.Print());

        Console.ReadLine();
    }
}