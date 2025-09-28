using KrtBank.Domain.Utils;
using Xunit;

namespace KrtBank.Tests.Utils;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("11144477735")]
    [InlineData("111.444.777-35")]
    [InlineData("111444777-35")]
    [InlineData("111.444777-35")]
    [InlineData("111.444.77735")]
    [InlineData("111 444 777 35")]
    [InlineData("111-444-777-35")]
    public void IsValid_ValidCpfs_ShouldReturnTrue(string cpf)
    {
        var result = CpfValidator.IsValid(cpf);
        Assert.True(result);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012")]
    [InlineData("abc.def.ghi-jk")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("111.444.777")]
    [InlineData("111444777")]
    [InlineData("11111111111")]
    [InlineData("00000000000")]
    [InlineData("12345678901")]
    public void IsValid_InvalidCpfs_ShouldReturnFalse(string cpf)
    {
        var result = CpfValidator.IsValid(cpf);
        Assert.False(result);
    }

    [Fact]
    public void IsValid_NullInput_ShouldReturnFalse()
    {
        var result = CpfValidator.IsValid(null!);
        Assert.False(result);
    }

    [Theory]
    [InlineData("111.444.777-35", "11144477735")]
    [InlineData("11144477735", "11144477735")]
    [InlineData("111 444 777 35", "11144477735")]
    [InlineData("111-444-777-35", "11144477735")]
    public void RemoveFormatting_ValidInputs_ShouldReturnOnlyDigits(string input, string expected)
    {
        var result = CpfValidator.RemoveFormatting(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("11144477735")]
    [InlineData("12345678901")]
    [InlineData("00000000000")]
    public void HasValidLength_ValidLength_ShouldReturnTrue(string cpf)
    {
        var result = CpfValidator.HasValidLength(cpf);
        Assert.True(result);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("123456789012")]
    [InlineData("")]
    public void HasValidLength_InvalidLength_ShouldReturnFalse(string cpf)
    {
        var result = CpfValidator.HasValidLength(cpf);
        Assert.False(result);
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("00000000000")]
    [InlineData("22222222222")]
    public void HasAllSameDigits_AllSameDigits_ShouldReturnTrue(string cpf)
    {
        var result = CpfValidator.HasAllSameDigits(cpf);
        Assert.True(result);
    }

    [Theory]
    [InlineData("11144477735")]
    [InlineData("12345678901")]
    public void HasAllSameDigits_DifferentDigits_ShouldReturnFalse(string cpf)
    {
        var result = CpfValidator.HasAllSameDigits(cpf);
        Assert.False(result);
    }

    [Theory]
    [InlineData("11144477735")]
    [InlineData("12345678909")]
    public void HasValidCheckDigits_ValidCheckDigits_ShouldReturnTrue(string cpf)
    {
        var result = CpfValidator.HasValidCheckDigits(cpf);
        Assert.True(result);
    }

    [Theory]
    [InlineData("11144477734")]
    [InlineData("12345678900")]
    public void HasValidCheckDigits_InvalidCheckDigits_ShouldReturnFalse(string cpf)
    {
        var result = CpfValidator.HasValidCheckDigits(cpf);
        Assert.False(result);
    }
}
