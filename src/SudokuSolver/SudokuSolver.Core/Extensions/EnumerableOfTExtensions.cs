using System.Linq;

namespace SudokuSolver.Core.Extensions
{
    public static class EnumerableOfTExtensions
    {
        public static bool In<T>(this T item, params T[] values)
        {
            return values.Contains(item);
        }
    }
}
