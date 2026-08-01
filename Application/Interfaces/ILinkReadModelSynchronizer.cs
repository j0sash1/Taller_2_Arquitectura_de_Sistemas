using Shortly.Domain.Entities;

namespace Shortly.Application.Interfaces;

// Called by command handlers after a write, to keep LinkReadModel in sync.
public interface ILinkReadModelSynchronizer
{
    Task UpsertAsync(Link link);

    Task RemoveAsync(long linkId);
}