namespace BE_Portfolio.Helpers;

public static class DateHelper
{
    public static string GetTimeAgo(DateTime? date)
    {
        if (!date.HasValue) return "Recently";
        var span = DateTime.UtcNow - date.Value;
        if (span.TotalHours < 1) return $"{span.Minutes} minutes ago";
        if (span.TotalHours < 24) return $"{Math.Floor(span.TotalHours)} hours ago";
        return $"{Math.Floor(span.TotalDays)} days ago";
    }
}
