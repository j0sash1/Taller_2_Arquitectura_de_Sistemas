using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.ReadRepositories;

public sealed class LinkReadModelSynchronizer : ILinkReadModelSynchronizer
{
    private readonly AppReadDbContext _context;

    public LinkReadModelSynchronizer(AppReadDbContext context)
    {
        _context = context;
    }

    public async Task UpsertAsync(Link link)
    {
        // Resolves the owner's email once here, so queries never have to.
        var ownerEmail = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == link.UserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync() ?? "";

        var existing = await _context.LinkReadModels.FindAsync(link.Id);

        if (existing is null)
        {
            _context.LinkReadModels.Add(new LinkReadModel(
                link.Id, link.Url, link.ShortUrl, link.Clicks, link.UserId, ownerEmail));
        }
        else
        {
            existing.Update(link.Url, link.ShortUrl, link.Clicks, ownerEmail);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(long linkId)
    {
        var existing = await _context.LinkReadModels.FindAsync(linkId);

        if (existing is not null)
        {
            _context.LinkReadModels.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}