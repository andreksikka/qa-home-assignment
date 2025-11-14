using CardValidation.Core.Enums;
using System.Linq;

namespace CardValidation.Core;

public static class CvcValidator
{
    public static bool IsValidForType(CardType type, string cvc)
    {
        if (string.IsNullOrWhiteSpace(cvc) || !cvc.All(char.IsDigit)) return false;

        return type switch
        {
            CardType.Visa            => cvc.Length == 3,
            CardType.MasterCard      => cvc.Length == 3,
            CardType.AmericanExpress => cvc.Length == 4,
            _ => false
        };
    }
}
