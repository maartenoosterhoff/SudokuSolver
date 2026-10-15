namespace SudokuSolver.NET.Parsers;

internal sealed class SudokuParsingException : Exception
{
    public SudokuParsingException() : base() { }
    public SudokuParsingException(string message) : base(message) { }
    public SudokuParsingException(string message, Exception e) : base(message, e) { }
}