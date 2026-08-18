using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Entities;
using SilentMoon.SharedKernel.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Search.Queries.SearchContent
{
    public class SearchContentQueryHandler : IQueryHandler<SearchContentQuery, List<SearchResultItemResponse>>
    {
        private readonly IUow _uow;
        private readonly IFileStorageService _fileStorage;
        private readonly IStringLocalizer<Messages> _localizer;

        public SearchContentQueryHandler(IUow uow,IFileStorageService fileStorage,IStringLocalizer<Messages> localizer)
        {
            _uow = uow;
            _fileStorage = fileStorage;
            _localizer = localizer;
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

            var translationRepo = _uow.GetRepository<Translation>();

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var items = await Task.WhenAll(matches.Select(async content => new SearchResultItemResponse
            {
                Id = content.Id,
                Title = translations.Localize(TranslationKeys.Content(content.Id, "Title"), content.Title),
                Category = _localizer.LocalizeCategory(content.Category),
                Duration = content.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct)
            }));

            return items.ToList();
        }
    }
}
