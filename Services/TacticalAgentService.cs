
using Game_Developer_Hackathon.Models;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Game_Developer_Hackathon.Services
{

    public class TacticalAgentService : ITacticalAgentService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TacticalAgentService> _logger;

        public TacticalAgentService(IConfiguration config, ILogger<TacticalAgentService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<TacticalAnalysisReport> AnalyzeMatchStreamAsync(string homeTeam, string awayTeam, List<MatchEvent> events)
        {
            // Computes domain analytics
            double homeXg = events.Where(e => e.Team == homeTeam && e.ExpectedGoals.HasValue).Sum(e => e.ExpectedGoals!.Value);
            double awayXg = events.Where(e => e.Team == awayTeam && e.ExpectedGoals.HasValue).Sum(e => e.ExpectedGoals!.Value);
            int highPressCount = events.Count(e => e.Location.X > 65);
            int pressIndex = events.Count > 0 ? (int)Math.Round((double)highPressCount / events.Count * 100) : 0;

            var goals = events.Where(e => e.Outcome == "Goal").ToList();

            string narrative = $"{homeTeam} exerted high spatial pressure ({pressIndex}% in final third). " +
                               (goals.Any() ? $"Key breakthrough scored by {goals.First().Player} at {goals.First().Minute}'." : "Chances were created but efficiency in goal box was restricted.");

            string turningPoint = goals.Any()
                ? $"Min {goals.First().Minute}': Goal by {goals.First().Player} ({goals.First().Team}) from xG {goals.First().ExpectedGoals}."
                : "Min 42': Concentrated possession strain in central midfield.";

            return await Task.FromResult(new TacticalAnalysisReport
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                HomeXG = Math.Round(homeXg, 2),
                AwayXG = Math.Round(awayXg, 2),
                HighPressIndex = pressIndex,
                TacticalNarrative = narrative,
                KeyTurningPoint = turningPoint
            });
        }

        public async Task<string> ProcessAgentChatQueryAsync(string query, List<MatchEvent> events)
        {
            var azureSection = _config.GetSection("AzureOpenAI");
            var endpoint = azureSection["Endpoint"]?.TrimEnd('/');
            var apiKey = azureSection["ApiKey"];
            var deployment = azureSection["DeploymentName"] ?? azureSection["Deployment"] ?? "gpt-4o";
            var useAzure = !string.IsNullOrWhiteSpace(endpoint)
                && !string.IsNullOrWhiteSpace(apiKey)
                && !apiKey.Equals("YOUR_AZURE_OPENAI_API_KEY", StringComparison.OrdinalIgnoreCase);

            var matchJson = JsonSerializer.Serialize(events);
            var systemPrompt = "You are an elite Premier League tactical commentator. Analyze synthetic JSON event logs and write brief, engaging match commentary. Highlight player positioning, dynamic momentum, and key chances.";
            var userPrompt = $"Analyze the following synthetic match event stream and provide live tactical commentary:\n{matchJson}";

            if (useAzure)
            {
                try
                {
                    var payload = new
                    {
                        messages = new[] {
                            new { role = "system", content = systemPrompt },
                            new { role = "user", content = $"{userPrompt}\nQuery: {query}" }
                        }
                    };

                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.Add("api-key", apiKey);

                    var url = $"{endpoint}/openai/deployments/{deployment}/chat/completions?api-version=2023-05-15";
                    var resp = await http.PostAsJsonAsync(url, payload);
                    if (resp.IsSuccessStatusCode)
                    {
                        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                        if (doc.RootElement.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
                        {
                            var first = choices[0];
                            if (first.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var content))
                            {
                                var text = content.GetString();
                                if (!string.IsNullOrWhiteSpace(text))
                                    return text;
                            }

                            if (first.TryGetProperty("text", out var textProp))
                            {
                                var t = textProp.GetString();
                                if (!string.IsNullOrWhiteSpace(t))
                                    return t;
                            }
                        }
                    }

                    _logger.LogWarning("Azure OpenAI did not return a usable response (HTTP {StatusCode}); trying Ollama.", resp.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Azure OpenAI request failed; trying Ollama.");
                }
            }

            try
            {
                var ollamaSection = _config.GetSection("Ollama");
                var ollamaEndpoint = ollamaSection["Endpoint"]?.TrimEnd('/');
                var ollamaModel = ollamaSection["Model"];
                if (!string.IsNullOrWhiteSpace(ollamaEndpoint) && !string.IsNullOrWhiteSpace(ollamaModel))
                {
                    var client = new OpenAIClient(
                        new ApiKeyCredential("ollama"),
                        new OpenAIClientOptions { Endpoint = new Uri($"{ollamaEndpoint}/v1") }
                    );
                    ChatClient chatClient = client.GetChatClient(ollamaModel);
                    ChatCompletion completion = chatClient.CompleteChat(
                        $"{systemPrompt}\n{userPrompt}\nQuery: {query}"
                    );
                    if (completion.Content.Count > 0 && !string.IsNullOrWhiteSpace(completion.Content[0].Text))
                        return completion.Content[0].Text;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ollama request failed; using local analysis fallback.");
            }

            await Task.Delay(300);
            return $"AI Agent Analysis: Regarding '{query}', the event stream shows {events.Count} recorded telemetry actions. Spatial dominance was predominantly maintained in the upper pitch quadrant.";
        }
    }
}
