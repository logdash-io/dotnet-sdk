namespace Logdash.Extensions;

public static class StringExtensions
{
    public static string WithColor(this string value, int r, int g, int b)
    {
        return $"\u001b[38;2;{r};{g};{b}m{value}\u001b[0m";
    }
}