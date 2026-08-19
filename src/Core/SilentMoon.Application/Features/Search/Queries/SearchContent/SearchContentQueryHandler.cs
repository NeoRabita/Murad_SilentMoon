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
            var trackRepo = _uow.GetRepository<Track>();
            var translationRepo = _uow.GetRepository<Translation>();

            var contents = await contentRepo.GetAllAsync(ct);
            var tracks = await trackRepo.GetAllAsync(ct);
            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var contentIdsWithMatchingTrack = tracks
                .Where(track => Matches(track.Title, query.Term) ||
                                 Matches(translations.Localize(TranslationKeys.For("Track", track.Id, "Title"), track.Title), query.Term))
                .Select(track => track.ContentId)
                .ToHashSet();

            var matches = contents
                .Where(content =>
                    Matches(content.Title, query.Term) ||
                    Matches(translations.Localize(TranslationKeys.For("Content", content.Id, "Title"), content.Title), query.Term) ||
                    Matches(content.Subtitle, query.Term) ||
                    Matches(translations.Localize(TranslationKeys.For("Content", content.Id, "Subtitle"), content.Subtitle), query.Term) ||
                    Matches(content.Category.ToString(), query.Term) ||
                    contentIdsWithMatchingTrack.Contains(content.Id))
                .OrderBy(x => x.SortOrder)
                .ToList();

            var items = await Task.WhenAll(matches.Select(async content => new SearchResultItemResponse
            {
                Id = content.Id,
                Title = translations.Localize(TranslationKeys.For("Content", content.Id, "Title"), content.Title),
                Category = _localizer.LocalizeCategory(content.Category),
                Duration = content.Duration,
                ThumbnailUrl = await _fileStorage.GetPresignedUrlAsync(MinioBucket.Media, content.ThumbnailUrl, ct)
            }));

            return items.ToList();
        }

        private static bool Matches(string value, string term) =>
            !string.IsNullOrEmpty(value) && value.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}
