using System;
using NodaTime;

namespace PingAI.DialogManagementService.Application.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertToLocal(this DateTime timeUtc, string timezoneName)
        {
            var instant = Instant.FromDateTimeUtc(timeUtc);
            var timeZone = DateTimeZoneProviders.Tzdb[timezoneName];
            var zonedDateTime = instant.InZone(timeZone);
            return zonedDateTime.ToDateTimeUnspecified();
        }
        
        public static DateTime ConvertToUtc(this DateTime dateTime, string timezoneName)
        {
            var localTime = LocalDateTime.FromDateTime(dateTime);
            var timeZone = DateTimeZoneProviders.Tzdb[timezoneName];
            var zonedTime = localTime.InZoneLeniently(timeZone);
            return zonedTime.ToDateTimeUtc();
        }
    }
}