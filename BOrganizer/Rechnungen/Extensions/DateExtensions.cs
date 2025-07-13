namespace Rechnungen.Extensions;

public static class DateExtensions
{
    public static DateTime ToDateTime(this DateOnly date) => date.ToDateTime(TimeOnly.MinValue);
}