using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GamingWebApp.Proxy;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Refit;

namespace GamingWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add registry
            var policyRegistry = services.AddPolicyRegistry();

            // Centrally stored policies
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1500));
            policyRegistry.Add("timeout", timeoutPolicy);

            // Options for external Web APIs and services
            services.Configure<LeaderboardApiOptions>(Configuration.GetSection("LeaderboardApiOptions"));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            ConfigureTypedClients(services);
            ConfigureSecurity(services);
            ConfigureTelemetry(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private void ConfigureTelemetry(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, ServiceNameInitializer>();
            var env = services.BuildServiceProvider().GetRequiredService<IHostingEnvironment>();
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.DeveloperMode = env.IsDevelopment();
                options.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
            });
        }

        private void ConfigureSecurity(IServiceCollection services)
        {
            services.AddHsts(
                options =>
                {
                    options.MaxAge = TimeSpan.FromDays(100);
                    options.IncludeSubDomains = true;
                    options.Preload = true;
                });
        }

        private void ConfigureTypedClients(IServiceCollection services)
        {
            services.AddHttpClient("WebAPIs", options =>
            {
                options.BaseAddress = new Uri(Configuration["LeaderboardApiOptions:BaseUrl"]);
                options.Timeout = TimeSpan.FromMilliseconds(15000);
                options.DefaultRequestHeaders.Add("ClientFactory", "Check");
            })
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(1500)))
            .AddTransientHttpErrorPolicy(p => p.RetryAsync(3))
            .AddTypedClient(client => RestService.For<ILeaderboardClient>(client));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(ILoggerFactory loggerFactory, IApplicationBuilder app, IHostingEnvironment env)
        {
            loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Information);
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
