using System;

namespace WebServiceDefault.Api.Cron
{
    public interface ICronJob<T>
    {
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
