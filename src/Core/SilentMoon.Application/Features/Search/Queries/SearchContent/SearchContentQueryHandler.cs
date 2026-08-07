using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Search.Queries.SearchContent
{
    public class SearchContentQueryHandler : IQueryHandler<SearchContentQuery, List<SearchResultItemResponse>>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;

        public SearchContentQueryHandler(IUow uow,IFileStorageService fileStorage)
        {
            _uow = uow;
            _fileStorage = fileStorage;
        }

        public async Task<Result<List<SearchResultItemResponse>>> Handle(SearchContentQuery query,CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query.Term))
            {
                return new List<SearchResultItemResponse>();
            }

            var contentRepo = _uow.GetRepository<Content>();

            var contents = await contentRepo.GetAllAsync(ct);

            var matches = contents
                .Where(x => x.Title != null && x.Title.Contains(query.Term, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.SortOrder)
                .ToList();

            var items = await Task.WhenAll(matches.Select(async content => new SearchResultItemResponse
            {
                Id = content.Id,
                Title = content.Title,
                Category = content.Category.ToString(),
                Duration = content.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct)
            }));

            return items.ToList();
        }
    }
}
