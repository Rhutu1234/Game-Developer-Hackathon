using Game_Developer_Hackathon.Models;

namespace Game_Developer_Hackathon.Services
{

    public class SyntheticDataService : ISyntheticDataService
    {
        private readonly string[] _homePlayers = { "Saka", "Odegaard", "Havertz", "Rice", "Saliba" };
        private readonly string[] _awayPlayers = { "Salah", "Diaz", "Nunez", "Mac Allister", "Van Dijk" };
        private readonly string[] _eventTypes = { "Pass", "Pass", "Pass", "Shot", "Tackle", "Interception" };

        public List<MatchEvent> GenerateMatchEvents(string homeTeam, string awayTeam, int count)
        {
            var events = new List<MatchEvent>();
            var rand = new Random();

            for (int i = 0; i < count; i++)
            {
                bool isHome = rand.NextDouble() > 0.45;
                string team = isHome ? homeTeam : awayTeam;
                string player = isHome ? _homePlayers[rand.Next(_homePlayers.Length)] : _awayPlayers[rand.Next(_awayPlayers.Length)];
                string type = _eventTypes[rand.Next(_eventTypes.Length)];

                int minute = (int)((double)i / count * 90);
                int second = rand.Next(0, 60);

                double startX = isHome ? rand.NextDouble() * 50 + 40 : rand.NextDouble() * 50 + 10;
                double startY = rand.NextDouble() * 90 + 5;

                string outcome = "Successful";
                double? xG = null;

                if (type == "Shot")
                {
                    double dist = Math.Sqrt(Math.Pow(100 - startX, 2) + Math.Pow(34 - startY, 2));
                    xG = Math.Max(0.02, Math.Round((1.0 - dist / 60.0), 2));
                    double shotOutcomeRoll = rand.NextDouble();
                    outcome = shotOutcomeRoll < 0.2 ? "Goal" : (shotOutcomeRoll < 0.6 ? "Saved" : "Off Target");
                }
                else if (type == "Pass")
                {
                    outcome = rand.NextDouble() > 0.2 ? "Successful" : "Unsuccessful";
                }

                events.Add(new MatchEvent
                {
                    Minute = minute,
                    Second = second,
                    Team = team,
                    Player = player,
                    Type = type,
                    Location = new PitchLocation { X = Math.Round(startX, 1), Y = Math.Round(startY, 1) },
                    EndLocation = type == "Pass" ? new PitchLocation { X = Math.Min(100, Math.Round(startX + (rand.NextDouble() * 20 - 5), 1)), Y = Math.Min(100, Math.Round(startY + (rand.NextDouble() * 20 - 10), 1)) } : null,
                    Outcome = outcome,
                    ExpectedGoals = xG
                });
            }

            return events;
        }
    }
}