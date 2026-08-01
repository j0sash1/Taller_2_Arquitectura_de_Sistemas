namespace Shortly.Application.DTOs;

public class StatsResponse
{
    public int TotalLinks { get; init; }
    public int TotalClicks { get; init; }
    public string? MostClickedShortUrl { get; init; }
    public string? MostClickedUrl { get; init; }
    public int MostClickedClicks { get; init; }
}