using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        AppDbContext db,
        ILinkReadModelSynchronizer readModelSync,
        string adminPassword)
    {
        if (await db.Users.AnyAsync())
            return;

        var user = new User("admin@shortly.disc.cl", adminPassword);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var links = new[]
        {
            new Link("https://learn.microsoft.com/aspnet/core", "aspnet", user.Id),
            new Link("https://learn.microsoft.com/ef/core", "efcore", user.Id),
            new Link("https://github.com", "github", user.Id)
        };

        db.Links.AddRange(links);
        await db.SaveChangesAsync();

        // Seeded links are a write too, so the read model needs them (item 5).
        foreach (var link in links)
        {
            await readModelSync.UpsertAsync(link);
        }
    }
}