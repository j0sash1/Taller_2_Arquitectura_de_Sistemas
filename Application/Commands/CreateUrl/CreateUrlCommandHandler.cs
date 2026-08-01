using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;

namespace Shortly.Application.Commands.CreateUrl;

public sealed class CreateUrlCommandHandler
{
    private readonly ILinkWriteRepository _repository;
    private readonly ILinkReadModelSynchronizer _readModelSync;
    private readonly ILogger<CreateUrlCommandHandler> _logger;

    public CreateUrlCommandHandler(
        ILinkWriteRepository repository,
        ILinkReadModelSynchronizer readModelSync,
        ILogger<CreateUrlCommandHandler> logger)
    {
        _repository = repository;
        _readModelSync = readModelSync;
        _logger = logger;
    }

    public async Task<LinkResponse> Handle(CreateUrlCommand command)
    {
        var shortUrl = Ulid.NewUlid()
            .ToString()[..12]
            .ToLowerInvariant();

        var link = new Link(
            command.Url,
            shortUrl,
            command.UserId);

        await _repository.AddAsync(link);

        await _repository.SaveChangesAsync();

        // Keeps the read model (item 5) up to date after the write.
        await _readModelSync.UpsertAsync(link);

        _logger.LogInformation(
            "Link created {ShortUrl}",
            link.ShortUrl);

        return LinkResponse.From(link);
    }
}