using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebServiceDefault.Api.Cron
{
    public static class CronServiceExtensions
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<ICronJob<T>> cronJob) where T : CronService
        {
            if (cronJob == null)
                throw new ArgumentNullException($"Bad {nameof(cronJob)} configured.");

            var initializedCronJob = new CronJob<T>();
            cronJob.Invoke(initializedCronJob);

            if (string.IsNullOrWhiteSpace(initializedCronJob.CronExpression))
                throw new ArgumentNullException($"{nameof(initializedCronJob.CronExpression)} is empty.");

            services.AddSingleton<ICronJob<T>>(initializedCronJob);
            services.AddHostedService<T>();

            return services;
        }
    }
}
