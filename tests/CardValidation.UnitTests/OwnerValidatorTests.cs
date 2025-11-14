using CardValidation.Core;
using FluentAssertions;
using Xunit;
namespace CardValidation.Core.Enums;

public class OwnerValidatorTests
{
    [Theory]
    [InlineData("Jane Doe", true)]
    [InlineData("Jane Maria Doe", true)]
    [InlineData("Jane", true)]
    [InlineData("Jane Maria Ann Doe", false)]
    [InlineData("John O'Neil", false)]
    [InlineData("John-Doe", false)]
    [InlineData("John 123", false)]
    public void Format(string owner, bool ok)
    {
        OwnerValidator.FormatOk(owner).Should().Be(ok);
    }

    [Fact]
    public void OwnerMustNotContainCardData()
    {
        OwnerValidator.HasNoCardData("John 4111111111111111", "4111111111111111", "123").Should().BeFalse();
        OwnerValidator.HasNoCardData("Amy 3714 Poe", "371449635398431", "1234").Should().BeFalse();
        OwnerValidator.HasNoCardData("Jane Doe", "4111111111111111", "123").Should().BeTrue();
    }
}
