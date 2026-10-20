using SudokuSolver.NET.Models;

namespace SudokuSolver.NET.Tests.Models;

public class BitSetTests
{
    [Test]
    public async Task Ctor_sets_default_values_correctly_to_true()
    {
        foreach (var value in new[] { false, true })
        {
            foreach (var size in Enumerable.Range(1, 127))
            {
                await TestValues(value, size);
            }
        }
    }

    private static async Task TestValues(bool value, int size)
    {
        // Arrange/act
        var bitSet = new BitSet(size, value);

        // Assert
        for (var i = 0; i < bitSet.Size; i++)
        {
            await Assert.That(bitSet[i]).IsEqualTo(value);
        }
    }

    [Test]
    public async Task Setter_sets_correct_bit()
    {
        // Arrange
        var set1 = new BitSet(10, false);

        // Act
        set1[2] = true;
        set1[5] = false;
        set1[5] = true;
        set1[8] = true;
        set1[8] = false;
        set1[9] = false;

        // Assert
        await Assert.That(set1[0]).IsFalse();
        await Assert.That(set1[1]).IsFalse();
        await Assert.That(set1[2]).IsTrue();
        await Assert.That(set1[3]).IsFalse();
        await Assert.That(set1[4]).IsFalse();
        await Assert.That(set1[5]).IsTrue();
        await Assert.That(set1[6]).IsFalse();
        await Assert.That(set1[7]).IsFalse();
        await Assert.That(set1[8]).IsFalse();
        await Assert.That(set1[9]).IsFalse();
    }

    //[Test]
    //public void AndOperator_sets_int_values_correctly()
    //{
    //    // Arrange
    //    var set1 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [5] = true,
    //        [8] = true
    //    };
    //    var set2 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [6] = true,
    //        [8] = true
    //    };

    //    // Act
    //    var set3 = set1 & set2;

    //    // Assert
    //    Assert.AreEqual(false, set3[0]);
    //    Assert.AreEqual(false, set3[1]);
    //    Assert.AreEqual(true, set3[2]);
    //    Assert.AreEqual(false, set3[3]);
    //    Assert.AreEqual(false, set3[4]);
    //    Assert.AreEqual(false, set3[5]);
    //    Assert.AreEqual(false, set3[6]);
    //    Assert.AreEqual(false, set3[7]);
    //    Assert.AreEqual(true, set3[8]);
    //    Assert.AreEqual(false, set3[9]);
    //}

    //[TestMethod]
    //public void OrOperator_sets_int_values_correctly()
    //{
    //    // Arrange
    //    var set1 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [5] = true,
    //        [8] = true
    //    };
    //    var set2 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [6] = true,
    //        [8] = true
    //    };

    //    // Act
    //    var set3 = set1 | set2;

    //    // Assert
    //    Assert.AreEqual(false, set3[0]);
    //    Assert.AreEqual(false, set3[1]);
    //    Assert.AreEqual(true, set3[2]);
    //    Assert.AreEqual(false, set3[3]);
    //    Assert.AreEqual(false, set3[4]);
    //    Assert.AreEqual(true, set3[5]);
    //    Assert.AreEqual(true, set3[6]);
    //    Assert.AreEqual(false, set3[7]);
    //    Assert.AreEqual(true, set3[8]);
    //    Assert.AreEqual(false, set3[9]);
    //}

    //[TestMethod]
    //public void XOrOperator_sets_int_values_correctly()
    //{
    //    // Arrange
    //    var set1 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [5] = true,
    //        [8] = true
    //    };
    //    var set2 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [6] = true,
    //        [8] = true
    //    };

    //    // Act
    //    var set3 = set1 ^ set2;

    //    // Assert
    //    Assert.AreEqual(false, set3[0]);
    //    Assert.AreEqual(false, set3[1]);
    //    Assert.AreEqual(false, set3[2]);
    //    Assert.AreEqual(false, set3[3]);
    //    Assert.AreEqual(false, set3[4]);
    //    Assert.AreEqual(true, set3[5]);
    //    Assert.AreEqual(true, set3[6]);
    //    Assert.AreEqual(false, set3[7]);
    //    Assert.AreEqual(false, set3[8]);
    //    Assert.AreEqual(false, set3[9]);
    //}

    //[TestMethod]
    //public void NotOperator_sets_int_values_correctly()
    //{
    //    // Arrange
    //    var set1 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [5] = true,
    //        [8] = true
    //    };

    //    // Act
    //    var set3 = !set1;

    //    // Assert
    //    Assert.AreEqual(true, set3[0]);
    //    Assert.AreEqual(true, set3[1]);
    //    Assert.AreEqual(false, set3[2]);
    //    Assert.AreEqual(true, set3[3]);
    //    Assert.AreEqual(true, set3[4]);
    //    Assert.AreEqual(false, set3[5]);
    //    Assert.AreEqual(true, set3[6]);
    //    Assert.AreEqual(true, set3[7]);
    //    Assert.AreEqual(false, set3[8]);
    //    Assert.AreEqual(true, set3[9]);
    //}

    //[TestMethod]
    //public void Count_counts_correctly()
    //{
    //    Assert.AreEqual(0, new BitSet(10, false).Count());
    //    Assert.AreEqual(10, new BitSet(10, true).Count());

    //    var set1 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [5] = true,
    //        [8] = true
    //    };
    //    Assert.AreEqual(3, set1.Count());
    //}

    //[TestMethod]
    //public void IsEmpty_returns_correct_value()
    //{
    //    Assert.AreEqual(true, new BitSet(10, false).IsEmpty());
    //    Assert.AreEqual(false, new BitSet(10, true).IsEmpty());

    //    var set1 = new BitSet(10, false)
    //    {
    //        [2] = true,
    //        [5] = true,
    //        [8] = true
    //    };
    //    Assert.AreEqual(false, set1.IsEmpty());
    //}
}
