using CardValidation.Core;
using FluentAssertions;
using Xunit;
namespace CardValidation.Core.Enums;

public class LuhnTests
{
    [Theory]
    [InlineData("4111111111111111", true)]
    [InlineData("4012888888881881", true)]
    [InlineData("5555555555554444", true)]
    [InlineData("5105105105105100", true)]
    [InlineData("371449635398431", true)]
    [InlineData("378282246310005", true)]
    [InlineData("4111111111111121", false)]
    [InlineData("1234567890123456", false)]
    public void Luhn_Works(string pan, bool ok)
    {
        Luhn.IsValid(pan).Should().Be(ok);
    }
}
