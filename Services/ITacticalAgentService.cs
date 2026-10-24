using Game_Developer_Hackathon.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Developer_Hackathon.Services
{
    public interface ITacticalAgentService
    {
        Task<TacticalAnalysisReport> AnalyzeMatchStreamAsync(string homeTeam, string awayTeam, List<MatchEvent> events);
        Task<string> ProcessAgentChatQueryAsync(string query, List<MatchEvent> events);
    }
}
