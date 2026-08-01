using Shortly.Application.Interfaces;

namespace Shortly.Application.Commands.DeleteUrl;

public sealed class DeleteUrlCommandHandler
{
    private readonly ILinkWriteRepository _repository;
    private readonly ILogger<DeleteUrlCommandHandler> _logger;

    public DeleteUrlCommandHandler(
        ILinkWriteRepository repository,
        ILogger<DeleteUrlCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(DeleteUrlCommand command)
    {
        var link = await _repository.GetByIdAsync(command.Id);

        if (link is null)
        {
            _logger.LogWarning("DeleteUrlCommand failed: no link found with id {Id}.", command.Id);
            throw new KeyNotFoundException($"No link found with id '{command.Id}'.");
        }

        await _repository.DeleteAsync(link);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Link deleted {Id}", command.Id);
    }
}