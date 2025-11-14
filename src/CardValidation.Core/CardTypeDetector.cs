using CardValidation.Core.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CardValidation.Core;


public static class CardTypeDetector
{
    // Normalize to digits only (strip spaces, hyphens and other non-digits)
    public static string Normalize(string input) =>
        new string(input.Where(char.IsDigit).ToArray());

    // Quick check: is the string all digits?
    public static bool IsNumeric(string s) => !string.IsNullOrEmpty(s) && s.All(char.IsDigit);

    // Any known card length?
    public static bool IsValidLengthAnyType(string digits) =>
        digits.Length == 13 || digits.Length == 15 || digits.Length == 16;

    // Length per specific type
    public static bool IsValidLengthForType(string digits, CardType type) =>
        type switch
        {
            CardType.Visa            => digits.Length is 13 or 16,
            CardType.MasterCard      => digits.Length == 16,
            CardType.AmericanExpress => digits.Length == 15,
            _ => false
        };

    /// <summary>
    /// Tries to detect Visa / MasterCard / AmericanExpress
    /// using digits-only PAN with strict BIN + length rules.
    /// Returns false if not numeric or length is not one of 13/15/16.
    /// </summary>
    public static bool TryDetect(string pan, out CardType type)
    {
        type = default;

        if (string.IsNullOrWhiteSpace(pan))
            return false;

        var digits = Normalize(pan);
        if (!IsNumeric(digits)) return false;
        if (!IsValidLengthAnyType(digits)) return false;

        // Visa: starts with 4 (13 or 16)
        if (digits.StartsWith("4") && (digits.Length == 13 || digits.Length == 16))
        {
            type = CardType.Visa;
            return true;
        }

        // MasterCard:
        //  - 51–55 (length 16)
        //  - 2221–2720 (length 16)
        if (digits.Length == 16)
        {
            // 51–55
            if (digits.StartsWith("51") || digits.StartsWith("52") || digits.StartsWith("53")
                || digits.StartsWith("54") || digits.StartsWith("55"))
            {
                type = CardType.MasterCard;
                return true;
            }

            // 2221–2720
            var bin4 = int.TryParse(digits.Substring(0, 4), out var first4) ? first4 : -1;
            var bin6 = int.TryParse(digits.Substring(0, 6), out var first6) ? first6 : -1;
            // Quick range check (use first 4 or 6—either is fine; first4 covers the range adequately here)
            if (bin4 >= 2221 && bin4 <= 2720)
            {
                type = CardType.MasterCard;
                return true;
            }
            // Some lists test specific MC examples like 222100..., 272099...
            // The 6-digit parse keeps parity with stricter BIN validators
            if (first6 >= 222100 && first6 <= 272099)
            {
                type = CardType.MasterCard;
                return true;
            }
        }

        // American Express: 34 or 37, length 15
        if (digits.Length == 15 && (digits.StartsWith("34") || digits.StartsWith("37")))
        {
            type = CardType.AmericanExpress;
            return true;
        }

        return false;
    }
}
