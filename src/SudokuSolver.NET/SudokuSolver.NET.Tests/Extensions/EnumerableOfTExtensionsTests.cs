using SudokuSolver.NET.Extensions;

namespace SudokuSolver.NET.Tests.Extensions;

public class EnumerableOfTExtensionsTests
{
    [Test]
    public async Task WhenParameterIsNotInCollection_ThenShouldReturnFalse()
    {
        // Arrange
        var values = new[] { 1, 2 };

        // Act
        var actual = 3.In(values);

        // Assert
        await Assert.That(actual).IsFalse();
    }

    [Test]
    public async Task WhenParameterIsInCollection_ThenShouldReturnTrue()
    {
        // Arrange
        var values = new[] { 1, 2 };

        // Act
        var actual = 1.In(values);

        // Assert
        await Assert.That(actual).IsTrue();
    }
}