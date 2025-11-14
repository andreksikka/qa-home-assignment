using System;

namespace CardValidation.Core;

public static class ExpiryValidator
{
    /// <summary>
    /// Accepts "MM/YY" or "MM/YYYY".
    /// Returns true if parsed and not expired (valid until the last day of month).
    /// </summary>
    public static bool TryParseAndIsNotExpired(
        string expiry,
        DateTime now,
        out int parsedMonth,
        out int parsedYear)
    {
        parsedMonth = 0;
        parsedYear  = 0;

        if (string.IsNullOrWhiteSpace(expiry))
            return false;

        var parts = expiry.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2) return false;

        if (!int.TryParse(parts[0], out var month) || month < 1 || month > 12) return false;
        if (!int.TryParse(parts[1], out var year)  || year < 1) return false;

        if (year < 100) year += 2000;

        parsedMonth = month;
        parsedYear  = year;

        DateTime lastValid;
        try
        {
            lastValid = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        }
        catch
        {
            return false;
        }

        return lastValid.Date >= now.Date;
    }
}
