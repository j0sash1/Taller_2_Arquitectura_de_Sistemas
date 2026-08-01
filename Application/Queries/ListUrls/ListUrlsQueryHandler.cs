using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;

namespace Shortly.Application.Queries.ListUrls;

public sealed class ListUrlsQueryHandler
{
    private readonly ILinkReadRepository _repository;

    public ListUrlsQueryHandler(ILinkReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LinkResponse>> Handle(ListUrlsQuery query)
    {
        var links = await _repository.GetAllAsync();

        return links
            .Select(LinkResponse.From)
            .ToList();
    }
}