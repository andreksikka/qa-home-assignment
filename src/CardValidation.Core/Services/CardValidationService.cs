using CardValidation.Core.Enums;
using CardValidation.Core.Services.Interfaces;

namespace CardValidation.Core.Services;

public class CardValidationService : ICardValidationService
{
    public bool ValidateOwner(string owner)
    {
        if (!OwnerValidator.IsPresent(owner))
            return false;

        return OwnerValidator.FormatOk(owner);
    }

    public bool ValidateIssueDate(string date)
    {
        return ExpiryValidator.TryParseAndIsNotExpired(
            date,
            DateTime.UtcNow,
            out _,
            out _
        );
    }
public bool ValidateNumber(string number)
{
    if (string.IsNullOrWhiteSpace(number))
        return false;

    var request = new CreditCardRequest(
        CardOwner: "Test",
        CardNumber: number,
        Expiry: "01/2099",
        CVC: "000"
    );

    var result = CreditCardValidator.Validate(request);

    return result.IsValid;
}


    public bool ValidateNumberAndCvcForType(string number, string cvc)
    {
        if (string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(cvc))
            return false;

        if (!CardTypeDetector.TryDetect(number, out var detectedType))
            return false;

        return CvcValidator.IsValidForType(detectedType, cvc);
    }

    public bool OwnerHasNoCardData(string owner, string number, string cvc)
    {
        return OwnerValidator.HasNoCardData(owner, number, cvc);
    }

    public CardType GetCardType(string number)
    {
        if (!CardTypeDetector.TryDetect(number, out var detected))
            throw new NotImplementedException("Unknown card type");

        return detected;
    }
}
