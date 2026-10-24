using System.Collections.Generic;

namespace Game_Developer_Hackathon.Models
{
    public class ChatRequest
    {
        public string Query { get; set; } = string.Empty;
        public List<MatchEvent> Events { get; set; } = new();
    }
}
