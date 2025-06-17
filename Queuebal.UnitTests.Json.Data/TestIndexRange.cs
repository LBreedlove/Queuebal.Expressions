using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Json.Data;

[TestClass]
public class TestIndexRange
{
    [TestMethod]
    public void test_build_when_index_range_is_empty_string()
    {
        // Arrange
        var indexRangeStr = " ";

        // Act
        Assert.Throws<ArgumentException>(() => IndexRange.Build(indexRangeStr));
    }

    [TestMethod]
    public void test_build_when_index_range_is_empty_range()
    {
        // Arrange
        var indexRangeStr = "[ ]";

        // Act
        var indexRange = IndexRange.Build(indexRangeStr);

        Assert.AreEqual(-1, indexRange.Start);
        Assert.AreEqual(-1, indexRange.End);

        Assert.IsTrue(indexRange.ContainsIndex(0));
        Assert.IsTrue(indexRange.ContainsIndex(1000));
    }

    [TestMethod]
    public void test_build_when_index_range_start_is_invalid()
    {
        // Arrange
        var indexRangeStr = "[a:]";

        // Act
        Assert.Throws<ArgumentException>(() => IndexRange.Build(indexRangeStr));
    }

    [TestMethod]
    public void test_build_when_single_item_index_range_is_invalid()
    {
        // Arrange
        var indexRangeStr = "[a]";

        // Act
        Assert.Throws<ArgumentException>(() => IndexRange.Build(indexRangeStr));
    }

    [TestMethod]
    public void test_build_when_index_range_end_is_invalid()
    {
        // Arrange
        var indexRangeStr = "[0:a]";

        // Act
        Assert.Throws<ArgumentException>(() => IndexRange.Build(indexRangeStr));
    }

    [TestMethod]
    public void test_build_for_single_item()
    {
        // Arrange
        var indexRangeStr = "[:]";

        var indexRange = IndexRange.Build(indexRangeStr);

        int index = 0;
        for (int i = 0; i < 10; ++i)
        {
            Assert.IsTrue(indexRange.ContainsIndex(index));
        }

        Assert.IsTrue(indexRange.ContainsIndex(10)); // All indices should be valid

        Assert.AreEqual(-1, indexRange.Start);
        Assert.AreEqual(-1, indexRange.End);
    }

    [TestMethod]
    public void test_build_when_start_is_missing()
    {
        // Arrange
        var indexRangeStr = "[:1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        Assert.IsTrue(indexRange.ContainsIndex(0));
        Assert.IsFalse(indexRange.ContainsIndex(1));

        Assert.AreEqual(-1, indexRange.Start);
        Assert.AreEqual(1, indexRange.End);
    }


    [TestMethod]
    public void test_build_when_end_is_missing()
    {
        // Arrange
        var indexRangeStr = "[1:]";

        var indexRange = IndexRange.Build(indexRangeStr);

        Assert.IsFalse(indexRange.ContainsIndex(0));
        Assert.IsTrue(indexRange.ContainsIndex(1));
        Assert.IsTrue(indexRange.ContainsIndex(2));

        Assert.AreEqual(1, indexRange.Start);
        Assert.AreEqual(-1, indexRange.End);
    }

    [TestMethod]
    public void test_build_when_single_index_specified()
    {
        // Arrange
        var indexRangeStr = "[1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        Assert.IsFalse(indexRange.ContainsIndex(0));
        Assert.IsTrue(indexRange.ContainsIndex(1));
        Assert.IsFalse(indexRange.ContainsIndex(2));

        Assert.AreEqual(1, indexRange.Start);
        Assert.AreEqual(2, indexRange.End);
    }

    [TestMethod]
    public void test_is_single_item_when_range_is_single_item()
    {
        // Arrange
        var indexRangeStr = "[1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsTrue(indexRange.IsSingleItem);
    }

    [TestMethod]
    public void test_is_single_item_when_range_is_all_items()
    {
        // Arrange
        var indexRangeStr = "[:]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsFalse(indexRange.IsSingleItem);
    }

    [TestMethod]
    public void test_is_single_item_when_range_has_same_start_and_end()
    {
        // Arrange
        var indexRangeStr = "[-1:-1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsFalse(indexRange.IsSingleItem);
    }

    [TestMethod]
    public void test_is_empty_when_range_is_empty()
    {
        // Arrange
        var indexRangeStr = "[1:1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsTrue(indexRange.IsEmpty);
    }

    [TestMethod]
    public void test_is_empty_when_range_is_not_empty()
    {
        // Arrange
        var indexRangeStr = "[1:2]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsFalse(indexRange.IsEmpty);
    }


    [TestMethod]
    public void test_is_empty_when_infinite_range()
    {
        // Arrange
        var indexRangeStr = "[-1:-1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsFalse(indexRange.IsEmpty);
    }

    [TestMethod]
    public void test_is_infinite_when_range_is_infinite()
    {
        // Arrange
        var indexRangeStr = "[1:-1]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsTrue(indexRange.IsInfinite);
    }


    [TestMethod]
    public void test_is_infinite_when_range_is_finite()
    {
        // Arrange
        var indexRangeStr = "[-1:3]";

        var indexRange = IndexRange.Build(indexRangeStr);

        // Act & Assert
        Assert.IsFalse(indexRange.IsInfinite);
    }
}