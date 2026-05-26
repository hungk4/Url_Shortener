using Xunit;
using UrlShortener.Helpers;


namespace UrlShortener.Tests;

public class Base62HelperTests
{
    [Fact]
    public void Encode_1_Returns_000001()
    {
        var result = Base62Helper.Encode(1);
        Assert.Equal("000001", result);
    }

    [Fact]
    public void Encode_0_Returns_000000()
    {
        var result = Base62Helper.Encode(0);
        Assert.Equal("000000", result);
    }

    [Theory]
    [InlineData(1, "000001")]
    [InlineData(62, "000010")]
    [InlineData(3844, "000100")]
    public void Encode_ReturnsExpected(long id, string expected)
    {
        var result = Base62Helper.Encode(id);
        Assert.Equal(expected, result);
    }
}