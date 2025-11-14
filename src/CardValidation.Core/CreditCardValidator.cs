using CardValidation.Core.Enums;

namespace CardValidation.Core;

public static class CreditCardValidator
{
    public static ValidationResult Validate(CreditCardRequest request)
    {
        var errors = new List<string>();

        // Validate owner
        if (!OwnerValidator.IsPresent(request.CardOwner))
            errors.Add("Owner is required.");

        if (!OwnerValidator.FormatOk(request.CardOwner))
            errors.Add("Owner format is invalid.");

        // Validate expiry
        if (!ExpiryValidator.TryParseAndIsNotExpired(
                request.Expiry,
                DateTime.UtcNow,
                out _,
                out _))
        {
            errors.Add("Expiry date is invalid or card is expired.");
        }

        // Validate number present
        if (string.IsNullOrWhiteSpace(request.CardNumber))
        {
            errors.Add("Card number is required.");
        }
        else
        {
            // Numeric check
            if (!request.CardNumber.All(char.IsDigit))
                errors.Add("Card number must contain digits only.");

            // Detect card type
            CardType? detectedType = null;

            if (CardTypeDetector.TryDetect(request.CardNumber, out var type))
                detectedType = type;
            else
                errors.Add("Unsupported or invalid card number.");

            // Length rules
            if (detectedType != null)
            {
                if (!IsLengthValidForType(request.CardNumber, detectedType.Value))
                    errors.Add("Invalid card number length.");

                // Luhn check
                if (!Luhn.IsValid(request.CardNumber))
                    errors.Add("Card number failed Luhn validation.");

                // CVC
                if (!CvcValidator.IsValidForType(detectedType.Value, request.CVC))
                    errors.Add("CVC is invalid for this card type.");
            }
        }

        // Final result
        if (errors.Count == 0)
        {
            return new ValidationResult(
                IsValid: true,
                CardType: CardTypeDetector.TryDetect(request.CardNumber, out var finalType) 
                    ? finalType 
                    : null,
                Errors: new List<string>()
            );
        }

        return new ValidationResult(
            IsValid: false,
            CardType: null,
            Errors: errors
        );
    }

    private static bool IsLengthValidForType(string number, CardType type)
    {
        return type switch
        {
            CardType.Visa => number.Length is 13 or 16,
            CardType.MasterCard => number.Length == 16,
            CardType.AmericanExpress => number.Length == 15,
            _ => false
        };
    }
}
