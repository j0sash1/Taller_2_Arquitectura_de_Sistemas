using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Application.Commands.CreateUrl;
using Shortly.Application.Queries.GetUrl;
using Shortly.Application.Queries.ListUrls;
namespace Shortly.Application.Services;

public sealed class LinkService : ILinkService
{
    private readonly ILogger<LinkService> _logger;
    private readonly ILinkRepository _linkRepository;

    // CQRS handlers
    private readonly CreateUrlCommandHandler _createUrlHandler;
    private readonly GetUrlQueryHandler _getUrlHandler;
    private readonly ListUrlsQueryHandler _listUrlsHandler;


    public LinkService(
        ILinkRepository linkRepository,
        ILogger<LinkService> logger,
        CreateUrlCommandHandler createUrlHandler,
        GetUrlQueryHandler getUrlHandler,
        ListUrlsQueryHandler listUrlsHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _linkRepository = linkRepository ?? throw new ArgumentNullException(nameof(linkRepository));

        // Initializes CQRS handlers
        _createUrlHandler = createUrlHandler ?? throw new ArgumentNullException(nameof(createUrlHandler));
        _getUrlHandler = getUrlHandler ?? throw new ArgumentNullException(nameof(getUrlHandler));
        _listUrlsHandler = listUrlsHandler ?? throw new ArgumentNullException(nameof(listUrlsHandler));
    }

    public async Task<LinkResponse> CreateLink(string url, long userId)
    {
        _logger.LogDebug("Dispatching CreateUrlCommand.");

        return await _createUrlHandler.Handle(
            new CreateUrlCommand(url, userId));
    }

    public async Task<LinkResponse> IncrementClicks(long linkId)
    {
        _logger.LogDebug("Incrementing clicks for linkId: {LinkId}", linkId);

        var link = await _linkRepository.GetByIdAsync(linkId);
        if (link is null)
        {
            _logger.LogWarning("IncrementClicks failed: No link found with id {LinkId}.", linkId);
            throw new KeyNotFoundException($"No link found with id '{linkId}'.");
        }

        link.IncrementClicks();
        await _linkRepository.SaveChangesAsync();

        _logger.LogInformation("Clicks incremented for linkId: {LinkId}. Total clicks: {Clicks}.", link.Id, link.Clicks);
        return LinkResponse.From(link);
    }

    public async Task<LinkResponse> GetLink(string shortUrl)
    {
        _logger.LogDebug("Dispatching GetUrlQuery.");

        return await _getUrlHandler.Handle(
            new GetUrlQuery(shortUrl));
    }

    public async Task<List<LinkResponse>> GetAllLinks()
    {
        _logger.LogDebug("Dispatching ListUrlsQuery.");

        return await _listUrlsHandler.Handle(
            new ListUrlsQuery());
    }

    public async Task<List<LinkResponse>> GetLinksByUserId(long userId)
    {
        _logger.LogDebug("Retrieving links for userId: {UserId}", userId);
        var links = await _linkRepository.GetByUserIdAsync(userId);

        _logger.LogInformation("Retrieved {Count} links for userId: {UserId}.", links.Count, userId);
        return links.Select(LinkResponse.From).ToList();
    }
}
