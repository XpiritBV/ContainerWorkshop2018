using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GamingWebApp.Proxy;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Timeout;
using Refit;

namespace GamingWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly IOptionsSnapshot<LeaderboardApiOptions> options;
        private readonly TelemetryClient telemetryClient;
        private readonly ILeaderboardClient proxy;

        public IndexModel(IOptionsSnapshot<LeaderboardApiOptions> options,
            TelemetryClient telemetryClient,
            ILeaderboardClient proxy, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<IndexModel>();
            this.options = options;
            this.telemetryClient = telemetryClient;
            this.proxy = proxy;
        }

        public IEnumerable<HighScore> Scores { get; private set; }

        public async Task OnGetAsync()
        {
            Scores = new List<HighScore>();
            try
            {
                //ILeaderboardClient proxy = RestService.For<ILeaderboardClient>(options.Value.BaseUrl);
                using (var operation = telemetryClient.StartOperation<RequestTelemetry>("LeaderboardWebAPICall"))
                {
                    try
                    {
                        Scores = await proxy.GetHighScores();
                    }
                    catch
                    {
                        operation.Telemetry.Success = false;
                        throw;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Http request failed.");
            }
            catch (TimeoutRejectedException ex)
            {
                logger.LogWarning(ex, "Timeout occurred when retrieving high score list.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unknown exception occurred while retrieving high score list");
            }
        }
    }
}
