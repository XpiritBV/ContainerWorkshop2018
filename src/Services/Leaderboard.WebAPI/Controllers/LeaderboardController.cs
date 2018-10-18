using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leaderboard.WebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prometheus.Client;

namespace Leaderboard.WebAPI.Controllers
{
    public class HighScore
    {
        public string Game { get; set; }
        public string Nickname { get; set; }
        public int Points { get; set; }
    }

    [Route("api/[controller]")]
    [Produces("application/xml", "application/json")]
    public class LeaderboardController : Controller
    {
        private readonly LeaderboardContext context;

        public LeaderboardController(LeaderboardContext context)
        {
            this.context = context;
        }

        // GET api/leaderboard
        /// <summary>
        /// Retrieve a list of leaderboard scores.
        /// </summary>
        /// <returns>List of high scores per game.</returns>
        /// <response code="200">The list was successfully retrieved.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HighScore>), 200)]
        public async Task<ActionResult<IEnumerable<HighScore>>> Get(int limit = 0)
        {
            var _counter = Metrics.CreateCounter("leaderboardcontroller_request_counter", "Counts number of requests on leaderboard controller", "count");
            _counter.Inc();

            var scores = from score in context.Scores
                         group new { score.Gamer.Nickname, score.Points } by score.Game into scoresPerGame
                         select new HighScore()
                         {
                             Game = scoresPerGame.Key,
                             Points = scoresPerGame.Max(e => e.Points),
                             Nickname = scoresPerGame.OrderByDescending(s => s.Points).First().Nickname
                         };
            return Ok(await scores.OrderBy(s => s.Points).ToListAsync());
        }
    }
}
