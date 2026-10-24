using Game_Developer_Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Developer_Hackathon.Services
{
    public interface ISyntheticDataService
    {
        List<MatchEvent> GenerateMatchEvents(string homeTeam, string awayTeam, int count);
    }
}
