using DotCruz.Garage.Domain.Entities.Vehicles;

namespace DotCruz.Garage.Domain.Services;

public static class OdometerCalculator
{
    public static double CalculateDailyAverage(
        IEnumerable<OdometerHistory> history,
        double fallbackEstimate,
        int windowDays = 90)
    {
        var sortedHistory = history
            .OrderByDescending(h => h.RecordedAt)
            .ToList();

        if (sortedHistory.Count < 2)
        {
            return fallbackEstimate;
        }

        var latest = sortedHistory.First();
        
        var limitDate = latest.RecordedAt.AddDays(-windowDays);
        var oldestInWindow = sortedHistory
            .Where(h => h.RecordedAt >= limitDate)
            .LastOrDefault();

        oldestInWindow ??= sortedHistory.Last();

        if (latest.Id == oldestInWindow.Id)
        {
            return fallbackEstimate;
        }

        double odometerDiff = latest.OdometerValue - oldestInWindow.OdometerValue;
        double daysDiff = (latest.RecordedAt - oldestInWindow.RecordedAt).TotalDays;

        if (odometerDiff <= 0 || daysDiff < 1)
            return fallbackEstimate;

        return odometerDiff / daysDiff;
    }
}
