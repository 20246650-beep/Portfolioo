using System.Text.RegularExpressions;

public static class Rules
{
    public static readonly Regex ImageUrl = new(@"^https?://\S+\.(jpg|jpeg|png|gif|webp)(\?\S*)?$", RegexOptions.IgnoreCase);
    public static readonly Regex Video = new(@"^https?://(www\.)?(youtube\.com/(watch\?v=|shorts/)|youtu\.be/)[A-Za-z0-9_-]{11}", RegexOptions.IgnoreCase);

    public static List<string> SplitUrls(string? text) =>
        (text ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}