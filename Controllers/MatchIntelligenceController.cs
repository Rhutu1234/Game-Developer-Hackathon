using Game_Developer_Hackathon.Models;
using Game_Developer_Hackathon.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Game_Developer_Hackathon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchIntelligenceController : ControllerBase
    {
        private readonly ISyntheticDataService _dataService;
        private readonly ITacticalAgentService _agentService;

        public MatchIntelligenceController(ISyntheticDataService dataService, ITacticalAgentService agentService)
        {
            _dataService = dataService;
            _agentService = agentService;
        }

        [HttpGet("generate-events")]
        public ActionResult<List<MatchEvent>> GenerateEvents([FromQuery] string homeTeam = "Arsenal FC", [FromQuery] string awayTeam = "Liverpool FC", [FromQuery] int count = 50)
        {
            var events = _dataService.GenerateMatchEvents(homeTeam, awayTeam, count);
            return Ok(events);
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<TacticalAnalysisReport>> AnalyzeMatch([FromQuery] string homeTeam, [FromQuery] string awayTeam, [FromBody] List<MatchEvent> events)
        {
            var report = await _agentService.AnalyzeMatchStreamAsync(homeTeam, awayTeam, events);
            return Ok(report);
        }

        [HttpPost("chat")]
        public async Task<ActionResult<string>> ChatWithAgent([FromBody] ChatRequest request)
        {
            var response = await _agentService.ProcessAgentChatQueryAsync(request.Query, request.Events);
            return Ok(new { Response = response });
        }
    }
}
