public class TacticalAnalysisReport
{
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public double HomeXG { get; set; }
    public double AwayXG { get; set; }
    public int HighPressIndex { get; set; }
    public string TacticalNarrative { get; set; } = string.Empty;
    public string KeyTurningPoint { get; set; } = string.Empty;
}