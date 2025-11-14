using CardValidation.Core;
using FluentAssertions;
using Xunit;
namespace CardValidation.Core.Enums;

public class CvcValidatorTests
{
    [Theory]
    [InlineData(CardType.Visa, "123", true)]
    [InlineData(CardType.MasterCard, "000", true)]
    [InlineData(CardType.AmericanExpress, "1234", true)]
    [InlineData(CardType.AmericanExpress, "123", false)]
    [InlineData(CardType.Visa, "12a", false)]
    public void Cvc_Per_Type(CardType type, string cvc, bool ok)
    {
        CvcValidator.IsValidForType(type, cvc).Should().Be(ok);
    }
}
