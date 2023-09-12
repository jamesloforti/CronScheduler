using System;

namespace WebServiceDefault.Api.Cron
{
    public class CronJob<T> : ICronJob<T>
    {
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
