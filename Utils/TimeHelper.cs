using System.Globalization;

namespace TransactionApi.Utils;

public static class TimeHelper
{
    public static bool IsTimestampValid(string isoUtc)
    {
        DateTime timestamp;
        if (!DateTime.TryParse(isoUtc, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out timestamp))
        {
            return false;
        }

        var serverTime = DateTime.Now;
        var diff = (serverTime - timestamp).Duration();

        return diff.TotalMinutes <= 5;
    }
}