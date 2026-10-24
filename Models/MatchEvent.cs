using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Developer_Hackathon.Models
{
    public class MatchEvent
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public int MatchId { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public string Team { get; set; } = string.Empty;

        // Primary canonical properties
        public string PlayerName { get; set; } = string.Empty;
        public PitchLocation StartLocation { get; set; } = new();
        public PitchLocation? EndLocation { get; set; }

        // Backwards-compatible aliases used by other project components
        public string Player
        {
            get => PlayerName;
            set => PlayerName = value;
        }

        public PitchLocation Location
        {
            get => StartLocation;
            set => StartLocation = value;
        }

        public string Type { get; set; } = string.Empty; // e.g., Pass, Shot, Tackle, Pressure
        public string Outcome { get; set; } = "Successful"; // Successful, Unsuccessful, Goal
        public double? ExpectedGoals { get; set; } // Optional metric for shots
    }
}
