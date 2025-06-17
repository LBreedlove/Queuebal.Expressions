using Queuebal.Json.Data;

namespace Queuebal.UnitTests.Json.Data;


[TestClass]
public class TestPathSegmenter
{
    [TestMethod]
    public void test_get_path_segments_when_simple_path()
    {
        // Arrange
        var path = "simple.path.test";

        // Act
        var segments = PathSegmenter.GetPathSegments(path).ToList();

        // Assert
        Assert.IsNotNull(segments);
        Assert.AreEqual(3, segments.Count);
        Assert.AreEqual("simple", segments[0]);
        Assert.AreEqual("path", segments[1]);
        Assert.AreEqual("test", segments[2]);
    }

    [TestMethod]
    public void test_get_path_segments_when_path_with_array_index()
    {
        // Arrange
        var path = "simple.path.items[0].value";

        // Act
        var segments = PathSegmenter.GetPathSegments(path).ToList();

        // Assert
        Assert.IsNotNull(segments);
        Assert.AreEqual(5, segments.Count);
        Assert.AreEqual("simple", segments[0]);
        Assert.AreEqual("path", segments[1]);
        Assert.AreEqual("items", segments[2]);
        Assert.AreEqual("[0]", segments[3]);
        Assert.AreEqual("value", segments[4]);
    }


    [TestMethod]
    public void test_get_path_segments_when_path_with_array_of_array()
    {
        // Arrange
        var path = "simple.path.items[0][:].value";

        // Act
        var segments = PathSegmenter.GetPathSegments(path).ToList();

        // Assert
        Assert.IsNotNull(segments);
        Assert.AreEqual(6, segments.Count);
        Assert.AreEqual("simple", segments[0]);
        Assert.AreEqual("path", segments[1]);
        Assert.AreEqual("items", segments[2]);
        Assert.AreEqual("[0]", segments[3]);
        Assert.AreEqual("[:]", segments[4]);
        Assert.AreEqual("value", segments[5]);
    }

    [TestMethod]
    public void test_get_path_segments_when_path_ends_with_array_index()
    {
        // Arrange
        var path = "simple.path.items[0][:]";

        // Act
        var segments = PathSegmenter.GetPathSegments(path).ToList();

        // Assert
        Assert.IsNotNull(segments);
        Assert.AreEqual(5, segments.Count);
        Assert.AreEqual("simple", segments[0]);
        Assert.AreEqual("path", segments[1]);
        Assert.AreEqual("items", segments[2]);
        Assert.AreEqual("[0]", segments[3]);
        Assert.AreEqual("[:]", segments[4]);
    }

    [TestMethod]
    public void test_get_path_segments_when_path_ends_without_closing_index()
    {
        // Arrange
        var path = "simple.path.items[0";

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_array_missing_closing_bracket()
    {
        // Arrange
        var path = "simple.path.items[0.value.";

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_closing_bracket_found_outside_array()
    {
        // Arrange
        var path = "simple.path.items[:].value]";

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_path_ends_with_dot()
    {
        // Arrange
        var path = "simple.path.items.";

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_array_index_followed_by_letter_at_end()
    {
        // Arrange
        var path = "simple.path.items[0]v";

        // Act & Assert
        Assert.Throws<Exception>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_consecutive_dots()
    {
        // Arrange
        var path = "a.b..c[0].d";

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_array_within_array()
    {
        // Arrange
        var path = "a.b.c[0[:]].d";

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => PathSegmenter.GetPathSegments(path).ToList());
    }

    [TestMethod]
    public void test_get_path_segments_when_complex_path()
    {
        // Arrange
        var path = "a.b.c[0].d[1:][:][].e.f[:2][0].g";

        // Act
        var segments = PathSegmenter.GetPathSegments(path).ToList();

        Assert.IsNotNull(segments);
        Assert.AreEqual(13, segments.Count);
        Assert.AreEqual("a", segments[0]);
        Assert.AreEqual("b", segments[1]);
        Assert.AreEqual("c", segments[2]);
        Assert.AreEqual("[0]", segments[3]);
        Assert.AreEqual("d", segments[4]);
        Assert.AreEqual("[1:]", segments[5]);
        Assert.AreEqual("[:]", segments[6]);
        Assert.AreEqual("[]", segments[7]);
        Assert.AreEqual("e", segments[8]);
        Assert.AreEqual("f", segments[9]);
        Assert.AreEqual("[:2]", segments[10]);
        Assert.AreEqual("[0]", segments[11]);
        Assert.AreEqual("g", segments[12]);
    }

    [TestMethod]
    public void test_get_path_segments_when_segment_starts_with_list()
    {
        // Arrange
        var path = "[:].a.b.c[:].d";

        // Act
        var segments = PathSegmenter.GetPathSegments(path).ToList();

        // Assert
        Assert.IsNotNull(segments);
        Assert.AreEqual(6, segments.Count);
        Assert.AreEqual("[:]", segments[0]);
        Assert.AreEqual("a", segments[1]);
        Assert.AreEqual("b", segments[2]);
        Assert.AreEqual("c", segments[3]);
        Assert.AreEqual("[:]", segments[4]);
        Assert.AreEqual("d", segments[5]);
    }
}