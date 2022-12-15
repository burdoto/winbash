namespace winbash.util;

public static class StringUtil
{
    public static string Trim(this string str, int len, bool rightBound = false, bool doFill = false, char fill = ' ')
    {
        str = str.Trim();
        var n = len - str.Length;
        if (n < 0) // remove n chars
            if (rightBound)
                str = str.Substring(0, len);
            else str = str.Substring(Math.Abs(n), str.Length + n);
        else if (n > 0 && doFill)
        { // pre-/append n chars
            var extra = string.Empty;
            for (int i = 0; i < n; i++)
                extra += fill;
            if (rightBound)
                str = extra + str;
            else str += extra;
        }
        return str;
    }
}