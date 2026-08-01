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
        return await _repository.GetAllAsync();
    }
}