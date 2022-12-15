using System.Globalization;

namespace winbash.util;

public class ByteUtil
{
    private static readonly char[] suffixes = new[] { 'K', 'M', 'G', 'T', 'P', 'E' };
    
    public static string ReadableAmount(double amount)
    {
        var i = -1;
        while (amount > 1000)
        {
            amount /= 1024;
            i += 1;
        }

        var num = amount < 10 ? $"{amount:#.#}" : Math.Round(amount, 0).ToString(CultureInfo.InvariantCulture);
        var suffix = i == -1 ? string.Empty : suffixes[i].ToString();
        return num + suffix;
    }
}