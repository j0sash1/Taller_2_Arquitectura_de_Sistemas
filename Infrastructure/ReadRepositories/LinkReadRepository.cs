using Microsoft.EntityFrameworkCore;
using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.ReadRepositories;

public sealed class LinkReadRepository : ILinkReadRepository
{
    private readonly AppReadDbContext _context;

    public LinkReadRepository(AppReadDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<LinkResponse?> GetByShortUrlAsync(string shortUrl)
    {
        var model = await _context.LinkReadModels
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.ShortUrl == shortUrl);

        return model is null ? null : LinkResponse.From(model);
    }

    public async Task<List<LinkResponse>> GetAllAsync()
    {
        var models = await _context.LinkReadModels.AsNoTracking().ToListAsync();
        return models.Select(LinkResponse.From).ToList();
    }

    public async Task<List<LinkResponse>> GetByUserIdAsync(long userId)
    {
        var models = await _context.LinkReadModels
            .AsNoTracking()
            .Where(l => l.UserId == userId)
            .ToListAsync();

        return models.Select(LinkResponse.From).ToList();
    }
}