using Game_Developer_Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Developer_Hackathon.Services
{
    public class SyntheticDataGenerator
    {
        private readonly List<string> _homePlayers = new() { "Saka", "Odegaard", "Rice", "Havertz", "Saliba" };
        private readonly List<string> _awayPlayers = new() { "Salah", "Diaz", "Mac Allister", "Van Dijk", "Alexander-Arnold" };

        public List<MatchEvent> GenerateSyntheticMatch(int matchId = 101, int totalEvents = 200)
        {
            var events = new List<MatchEvent>();
            var random = new Random();

            for (int i = 0; i < totalEvents; i++)
            {
                int minute = (int)((double)i / totalEvents * 90);
                int second = random.Next(0, 60);
                bool isHomeTeam = random.NextDouble() > 0.45;

                string team = isHomeTeam ? "Arsenal FC" : "Liverpool FC";
                string player = isHomeTeam
                    ? _homePlayers[random.Next(_homePlayers.Count)]
                    : _awayPlayers[random.Next(_awayPlayers.Count)];

                string[] eventTypes = { "Pass", "Pass", "Pass", "Tackle", "Pressure", "Shot" };
                string eventType = eventTypes[random.Next(eventTypes.Length)];

                double startX = isHomeTeam ? random.NextDouble() * 50 + 40 : random.NextDouble() * 50 + 10;
                double startY = random.NextDouble() * 100;

                var matchEvent = new MatchEvent
                {
                    MatchId = matchId,
                    Minute = minute,
                    Second = second,
                    Team = team,
                    PlayerName = player,
                    Type = eventType,
                    StartLocation = new PitchLocation { X = Math.Round(startX, 1), Y = Math.Round(startY, 1) },
                    Outcome = (eventType == "Shot" && random.NextDouble() > 0.85) ? "Goal" : "Successful"
                };

                if (eventType == "Shot")
                {
                    // Basic xG calculation based on distance to goal
                    double distanceToGoal = Math.Sqrt(Math.Pow(100 - startX, 2) + Math.Pow(50 - startY, 2));
                    matchEvent.ExpectedGoals = Math.Round(Math.Max(0.01, 1.0 - (distanceToGoal / 50.0)), 2);
                }

                events.Add(matchEvent);
            }

            return events;
        }
    }
}
