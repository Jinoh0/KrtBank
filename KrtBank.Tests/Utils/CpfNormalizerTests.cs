using KrtBank.Application.Utils;
using Xunit;

namespace KrtBank.Tests.Utils;

public class CpfNormalizerTests
{
    [Theory]
    [InlineData("11144477735", "111.444.777-35")]
    [InlineData("111.444.777-35", "111.444.777-35")]
    [InlineData("111444777-35", "111.444.777-35")]
    [InlineData("111.444777-35", "111.444.777-35")]
    [InlineData("111.444.77735", "111.444.777-35")]
    [InlineData("111 444 777 35", "111.444.777-35")]
    [InlineData("111-444-777-35", "111.444.777-35")]
    [InlineData("111.444.777.35", "111.444.777-35")]
    public void Normalize_ValidFormats_ShouldReturnStandardFormat(string input, string expected)
    {
        var result = CpfNormalizer.Normalize(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012")]
    [InlineData("abc.def.ghi-jk")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("111.444.777")]
    [InlineData("111444777")]
    public void Normalize_InvalidFormats_ShouldThrowArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => CpfNormalizer.Normalize(input));
    }

    [Fact]
    public void Normalize_NullInput_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CpfNormalizer.Normalize(null!));
    }
}
