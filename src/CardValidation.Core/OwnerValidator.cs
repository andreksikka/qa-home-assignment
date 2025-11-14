using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CardValidation.Core;

public static class OwnerValidator
{
    public static bool IsPresent(string? owner) =>
        !string.IsNullOrWhiteSpace(owner);

    /// <summary>
    /// 1–3 words, letters only (no hyphens, apostrophes, digits).
    /// </summary>
    public static bool FormatOk(string owner)
    {
        // Trim, collapse inner whitespace to single spaces
        var norm = Regex.Replace(owner.Trim(), @"\s+", " ");
        // Only letters and spaces, 1–3 words
        if (!Regex.IsMatch(norm, @"^[A-Za-z ]+$")) return false;
        var words = norm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length is >= 1 and <= 3;
    }

    /// <summary>
    /// Must NOT contain PAN/CVC or any long digit sequence.
    /// We flag if:
    ///  - owner contains any 4+ consecutive digits
    ///  - owner contains a subsequence of PAN of length >= 4
    ///  - owner contains the exact CVC
    /// </summary>
    public static bool HasNoCardData(string owner, string? pan, string? cvc)
    {
        var o = owner ?? string.Empty;

        // 4+ digits in a row in owner
        if (Regex.IsMatch(o, @"\d{4,}")) return false;

        // CVC present as-is (if provided)
        if (!string.IsNullOrWhiteSpace(cvc) && o.Contains(cvc, StringComparison.Ordinal))
            return false;

        // Any 4+ digit subsequence from PAN
        if (!string.IsNullOrWhiteSpace(pan))
        {
            var digits = new string(pan.Where(char.IsDigit).ToArray());
            for (int len = 4; len <= 8 && len <= digits.Length; len++)
            {
                for (int i = 0; i + len <= digits.Length; i++)
                {
                    var sub = digits.Substring(i, len);
                    if (o.Contains(sub, StringComparison.Ordinal))
                        return false;
                }
            }
        }

        return true;
    }
}
