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

        var pointsInWindow = sortedHistory
            .Where(h => h.RecordedAt >= limitDate)
            .ToList();

        OdometerHistory oldest;
        if (pointsInWindow.Count >= 2)
        {
            oldest = pointsInWindow.Last();
        }
        else
        {
            oldest = sortedHistory.Last();
        }

        if (latest.Id == oldest.Id || latest.RecordedAt == oldest.RecordedAt)
        {
            return fallbackEstimate;
        }

        double odometerDiff = latest.OdometerValue - oldest.OdometerValue;
        double daysDiff = (latest.RecordedAt - oldest.RecordedAt).TotalDays;

        if (odometerDiff <= 0 || daysDiff < 1)
        {
            return fallbackEstimate;
        }

        return Math.Round(odometerDiff / daysDiff, 1);
    }
}
