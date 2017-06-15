using System;
using System.Linq;

namespace SudokuSolver.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this object obj) where TAttribute : Attribute
        {
            if (obj == null)
                return null;

            return obj.GetType().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault();
        }
    }
}
