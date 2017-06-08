using System;
using SudokuSolver.Core.Model;

namespace SudokuSolver.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var set = new BitSet(10, false);
            var setb = !set;
            Console.WriteLine(setb.GetType().FullName);
            set[9] = false;
            //set[10] = false;
            Console.WriteLine("Hello World!");
            Console.ReadLine();

        }
    }
}
