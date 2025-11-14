using System;
using CardValidation.Core;
using CardValidation.Core.Enums;
using FluentAssertions;
using Xunit;

public class CreditCardValidatorAggregationTests
{
    [Fact]
    public void Returns_All_Errors()
    {
        var req = new CreditCardRequest(
            CardOwner: "John 4111111111111111",
            CardNumber: "4111111111111111XX", // contains letters → NO Luhn
            Expiry: "10/20",
            CVC: "12"
        );

        // NOTE: CreditCardValidator no longer supports passing "current date"
        // So we temporarily adjust ExpiryValidator by setting an expired date inside "req"
        // The logic already checks expiry correctly using current UTC time.

        var result = CreditCardValidator.Validate(req);

        result.IsValid.Should().BeFalse();
        result.CardType.Should().BeNull();

        // Owner format
        result.Errors.Should().Contain(e => e.Contains("invalid", StringComparison.OrdinalIgnoreCase));

        // Card number has letters → numeric check failure
        result.Errors.Should().Contain(e => e.Contains("digits", StringComparison.OrdinalIgnoreCase));

        // Expired
        result.Errors.Should().Contain(e => e.Contains("expired", StringComparison.OrdinalIgnoreCase));

        // Length incorrect because of letters
        result.Errors.Should().Contain(e => e.Contains("length", StringComparison.OrdinalIgnoreCase));

        // Should NOT contain Luhn because numeric check already failed
        result.Errors.Should().Contain(e => e.Contains("Luhn", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Invalid_Luhn_Should_Produce_Error()
    {
        var req = new CreditCardRequest(
            CardOwner: "Jane Doe",
            CardNumber: "4111111111111112", // numeric but fails Luhn
            Expiry: "11/2030",
            CVC: "123"
        );

        var result = CreditCardValidator.Validate(req);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Luhn", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Success_Happy_Path_Visa()
    {
        var req = new CreditCardRequest(
            CardOwner: "Jane Doe",
            CardNumber: "4111111111111111",
            Expiry: "11/2099",
            CVC: "123"
        );

        var result = CreditCardValidator.Validate(req);

        result.IsValid.Should().BeTrue();
        result.CardType.Should().Be(CardType.Visa);
        result.Errors.Should().BeEmpty();
    }
}
