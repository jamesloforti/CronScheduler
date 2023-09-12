using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebServiceDefault.Common.Settings;
using WebServiceDefault.Library.Clients.Interfaces;

namespace WebServiceDefault.Api.Cron.CronJobs
{
    public class ProcessRecords : CronService
    {
        private readonly ILogger<ProcessRecords> _logger;
        private readonly IGenericHttpClient _genericHttpClient;
        private readonly AppSettings _appSettings;

        public ProcessRecords(
            ILogger<ProcessRecords> logger,
            AppSettings appSettings,
            IGenericHttpClient genericClient,
            ICronJob<ProcessRecords> cronJob)
            : base(cronJob.CronExpression, cronJob.TimeZoneInfo)
        {
            _logger = logger;
            _genericHttpClient = genericClient;
            _appSettings = appSettings;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, $"{nameof(ProcessRecords)} started.");
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                _logger.Log(LogLevel.Information, $"{nameof(CronService)} triggered:", new Dictionary<string, object>
                {
                    { nameof(CronService), nameof(ProcessRecords) }
                });
                await _genericHttpClient.GetAsync<string>($"{_appSettings.DownstreamServiceUrl}/{nameof(ProcessRecords)}");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, $"{nameof(CronService)} failed:", new Dictionary<string, object>
                {
                    { nameof(CronService), nameof(ProcessRecords) }
                });
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, $"{nameof(ProcessRecords)} is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
