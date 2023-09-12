using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Http.Headers;
using System.Net.Mime;
using WebServiceDefault.Api.Cron;
using WebServiceDefault.Api.Cron.CronJobs;
using WebServiceDefault.Api.HealthChecks;
using WebServiceDefault.Common.Settings;
using WebServiceDefault.Library.Clients;
using WebServiceDefault.Library.Clients.Interfaces;
using WebServiceDefault.Library.Providers;
using WebServiceDefault.Library.Providers.Interfaces;

namespace WebServiceDefault.Api
{
    public class Startup
	{
		private readonly IConfiguration _config;
		private readonly AppSettings _appSettings;
		private readonly AuthSettings _authSettings;
		private readonly CronSettings _cronSettings;

		public Startup(IConfiguration configuration)
		{
			_config = configuration;
			_appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
			_authSettings = configuration.GetSection("AuthSettings").Get<AuthSettings>();
			_cronSettings = configuration.GetSection("CronSettings").Get<CronSettings>();
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddHttpContextAccessor();
			services.AddSwaggerGen(c =>
			{
				c.EnableAnnotations();
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = _appSettings.ApplicationName,
					Description = "Web service.",
					Contact = new OpenApiContact()
					{
						Name = "James LoForti",
						Email = "jamesloforti@gmail.com",
						Url = new Uri("http://www.jimmyloforti.com")
					},
					License = new OpenApiLicense
					{
						Name = "GNU Affero General Public License",
						Url = new Uri("https://www.gnu.org/licenses/"),
					}
				});
			});
			services.AddHealthChecks()
				.AddCheck<GeneralHealth>("GeneralHealth");
			services.AddSingleton(_appSettings);
			services.AddSingleton(_authSettings);
			services.AddSingleton<IExampleProvider, ExampleProvider>();
			services.AddSingleton<ISimpleAuthClient<AuthHttpClient>, AuthHttpClient>();
			services.AddHttpClient<IGenericHttpClient, GenericHttpClient>((serviceProvider, httpClient) =>
			{
				var headers = httpClient.DefaultRequestHeaders;
				headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
				httpClient.Timeout = TimeSpan.FromMinutes(_appSettings.HttpClientTimeoutMinutes);
			});

			services.AddCronJob<ProcessRecords>(c =>
			{
				c.TimeZoneInfo = TimeZoneInfo.Local;
				c.CronExpression = _cronSettings.ProcessRecords;
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
				app.UseHsts();

			app.UseStaticFiles();
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHealthChecks("/GeneralHealth");
			});
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint(_appSettings!.SwaggerJsonUrl, _appSettings.ApplicationName);
				c.DocumentTitle = _appSettings.ApplicationName;
				c.RoutePrefix = "swagger";
				c.EnableTryItOutByDefault();
			});
		}
	}
}
