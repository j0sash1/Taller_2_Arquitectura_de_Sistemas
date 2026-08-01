using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;
using Shortly.Application.Commands.CreateUrl;
using Shortly.Application.Commands.DeleteUrl;
using Shortly.Application.Queries.GetUrl;
using Shortly.Application.Queries.ListUrls;
using Shortly.Application.Queries.GetStats;
namespace Shortly.Application.Services;

public sealed class LinkService : ILinkService
{
    private readonly ILogger<LinkService> _logger;
    private readonly ILinkWriteRepository _writeRepository;
    private readonly ILinkReadRepository _readRepository;
    private readonly ILinkReadModelSynchronizer _readModelSync;

    // CQRS handlers
    private readonly CreateUrlCommandHandler _createUrlHandler;
    private readonly DeleteUrlCommandHandler _deleteUrlHandler;
    private readonly GetUrlQueryHandler _getUrlHandler;
    private readonly ListUrlsQueryHandler _listUrlsHandler;
    private readonly GetStatsQueryHandler _getStatsHandler;


    public LinkService(
        ILinkWriteRepository writeRepository,
        ILinkReadRepository readRepository,
        ILinkReadModelSynchronizer readModelSync,
        ILogger<LinkService> logger,
        CreateUrlCommandHandler createUrlHandler,
        DeleteUrlCommandHandler deleteUrlHandler,
        GetUrlQueryHandler getUrlHandler,
        ListUrlsQueryHandler listUrlsHandler,
        GetStatsQueryHandler getStatsHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        _readModelSync = readModelSync ?? throw new ArgumentNullException(nameof(readModelSync));

        // Initializes CQRS handlers
        _createUrlHandler = createUrlHandler ?? throw new ArgumentNullException(nameof(createUrlHandler));
        _deleteUrlHandler = deleteUrlHandler ?? throw new ArgumentNullException(nameof(deleteUrlHandler));
        _getUrlHandler = getUrlHandler ?? throw new ArgumentNullException(nameof(getUrlHandler));
        _listUrlsHandler = listUrlsHandler ?? throw new ArgumentNullException(nameof(listUrlsHandler));
        _getStatsHandler = getStatsHandler ?? throw new ArgumentNullException(nameof(getStatsHandler));
    }

    public async Task<LinkResponse> CreateLink(string url, long userId)
    {
        _logger.LogDebug("Dispatching CreateUrlCommand.");

        return await _createUrlHandler.Handle(
            new CreateUrlCommand(url, userId));
    }

    public async Task DeleteLink(long linkId)
    {
        _logger.LogDebug("Dispatching DeleteUrlCommand.");

        await _deleteUrlHandler.Handle(
            new DeleteUrlCommand(linkId));
    }

    public async Task<LinkResponse> IncrementClicks(long linkId)
    {
        _logger.LogDebug("Incrementing clicks for linkId: {LinkId}", linkId);

        // Write repo: loads a tracked entity to mutate it.
        var link = await _writeRepository.GetByIdAsync(linkId);
        if (link is null)
        {
            _logger.LogWarning("IncrementClicks failed: No link found with id {LinkId}.", linkId);
            throw new KeyNotFoundException($"No link found with id '{linkId}'.");
        }

        link.IncrementClicks();
        await _writeRepository.SaveChangesAsync();

        // Keeps the read model (item 5) in sync with the new click count.
        await _readModelSync.UpsertAsync(link);

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

        // Read-only: goes through the read repo, never the write one.
        var links = await _readRepository.GetByUserIdAsync(userId);

        _logger.LogInformation("Retrieved {Count} links for userId: {UserId}.", links.Count, userId);
        return links;
    }

    public async Task<StatsResponse> GetStats(long userId)
    {
        _logger.LogDebug("Dispatching GetStatsQuery.");

        return await _getStatsHandler.Handle(
            new GetStatsQuery(userId));
    }
}