using Shortly.Application.DTOs;

namespace Shortly.Application.Interfaces;

// Used only by query handlers. Reads from the LinkReadModel table (item 5),
// not from "links" directly, and returns DTOs — no domain entities.
public interface ILinkReadRepository
{
    Task<LinkResponse?> GetByShortUrlAsync(string shortUrl);

    Task<List<LinkResponse>> GetAllAsync();

    Task<List<LinkResponse>> GetByUserIdAsync(long userId);
}