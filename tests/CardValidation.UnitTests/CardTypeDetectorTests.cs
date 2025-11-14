using CardValidation.Core.Enums;
using CardValidation.Core;
using FluentAssertions;
using Xunit;
namespace CardValidation.UnitTests;

public class CardTypeDetectorTests
{
    [Theory]
    [InlineData("4111111111111111", CardType.Visa)]
    [InlineData("4012888888881881", CardType.Visa)]
    [InlineData("5555555555554444", CardType.MasterCard)]
    [InlineData("5105105105105100", CardType.MasterCard)]
    [InlineData("2221000000000009", CardType.MasterCard)]
    [InlineData("2720990000000000", CardType.MasterCard)]
    [InlineData("371449635398431", CardType.AmericanExpress)]
    [InlineData("378282246310005", CardType.AmericanExpress)]
    public void Detects_Known_Types(string pan, CardType type)
    {
        CardTypeDetector.TryDetect(pan, out var t).Should().BeTrue();
        t.Should().Be(type);
    }

    [Theory]
    [InlineData("1234567890123")]
    [InlineData("9111111111111111")]
    public void Unknown_ReturnsFalse(string pan)
    {
        CardTypeDetector.TryDetect(pan, out var _).Should().BeFalse();
    }
}
