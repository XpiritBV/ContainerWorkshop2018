using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.WebAPI.Models;

namespace Leaderboard.WebAPI.Infrastructure
{
    public class DbInitializer
    {
        public async static Task Initialize(LeaderboardContext context)
        {
            context.Database.EnsureCreated();
            if (context.Gamers.Any())
            {
                return;
            }
            var gamer = context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "LX360", Scores = new List<Score>() { new Score() { Points = 1337, Game = "Pacman" } } });
            
            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "Kn3luZ" });
            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "BeyondDotNET" });
            await context.SaveChangesAsync();
        }
    }
}
