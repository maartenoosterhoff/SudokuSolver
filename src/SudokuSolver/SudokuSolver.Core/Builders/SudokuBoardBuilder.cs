using System;
using SudokuSolver.Core.Extensions;
using SudokuSolver.Core.Models;

namespace SudokuSolver.Core.Builders
{
    public interface ISudokuBoardBuilder
    {
        SudokuBoard Build(SudokuType sudokuType);
    }

    public class SudokuBoardBuilderProxy : ISudokuBoardBuilder
    {
        public SudokuBoardBuilderProxy(ISudokuBoardBuilder[] builders)
        {
            if (builders == null)
                throw new ArgumentNullException(nameof(builders));

            _builders = builders;
        }

        private readonly ISudokuBoardBuilder[] _builders;

        public SudokuBoard Build(SudokuType sudokuType)
        {
            foreach (var builder in _builders)
            {
                var sudokuTypeAttribute = builder.GetCustomAttribute<SudokuTypeAttribute>();
                if (sudokuTypeAttribute?.SudokuType == sudokuType)
                {
                    return builder.Build(sudokuType);
                }
            }

            return null;
        }
    }
}
