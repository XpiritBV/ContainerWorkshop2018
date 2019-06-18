using GamingWebApp.Proxy;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GamingWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly TelemetryClient telemetryClient;
        private readonly ILeaderboardClient proxy;

        public IndexModel(TelemetryClient telemetryClient,
            ILeaderboardClient proxy, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<IndexModel>();
            this.telemetryClient = telemetryClient;
            this.proxy = proxy;
        }

        public IEnumerable<HighScore> Scores { get; private set; }

        public async Task OnGetAsync()
        {
            Scores = new List<HighScore>();
            try
            {
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
