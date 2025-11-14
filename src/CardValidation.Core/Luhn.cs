using System;
using System.Linq;

namespace CardValidation.Core;

public static class Luhn
{
    public static bool IsValid(string panRaw)
    {
        if (string.IsNullOrWhiteSpace(panRaw))
            return false;

        // Luhn must fail if any non-digit is present
        if (!panRaw.All(char.IsDigit))
            return false;

        int sum = 0;
        bool doubleIt = false;

        for (int i = panRaw.Length - 1; i >= 0; i--)
        {
            int d = panRaw[i] - '0';
            if (doubleIt)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleIt = !doubleIt;
        }

        return sum % 10 == 0;
    }
}