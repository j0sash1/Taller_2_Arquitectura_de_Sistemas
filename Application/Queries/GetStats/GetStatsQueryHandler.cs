using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;

namespace Shortly.Application.Queries.GetStats;

public sealed class GetStatsQueryHandler
{
    private readonly ILinkReadRepository _repository;

    public GetStatsQueryHandler(ILinkReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<StatsResponse> Handle(GetStatsQuery query)
    {
        var links = await _repository.GetByUserIdAsync(query.UserId);

        var mostClicked = links
            .OrderByDescending(l => l.Clicks)
            .FirstOrDefault();

        return new StatsResponse
        {
            TotalLinks = links.Count,
            TotalClicks = links.Sum(l => l.Clicks),
            MostClickedShortUrl = mostClicked?.ShortUrl,
            MostClickedUrl = mostClicked?.Url,
            MostClickedClicks = mostClicked?.Clicks ?? 0
        };
    }
}