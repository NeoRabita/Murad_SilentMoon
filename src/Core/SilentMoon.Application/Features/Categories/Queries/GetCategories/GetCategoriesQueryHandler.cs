using Application.Abstractions.Messaging;
using SilentMoon.Application.Common;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, DataEnvelope<CategoryResponse>>
    {
        private readonly IUow _uow;

        public GetCategoriesQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<DataEnvelope<CategoryResponse>>> Handle(GetCategoriesQuery query, CancellationToken ct)
        {
            var categoryRepo = _uow.GetRepository<Category>();
            var translationRepo = _uow.GetRepository<Translation>();

            var categories = (await categoryRepo.GetAllAsync(ct)).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query.Type) &&
                Enum.TryParse<ContentCategory>(query.Type, ignoreCase: true, out var type))
            {
                categories = categories.Where(x => x.Type == type);
            }

            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var items = categories
                .OrderBy(x => x.SortOrder)
                .Select(category => new CategoryResponse
                {
                    Id = category.Id,
                    Slug = category.Slug,
                    Title = translations.Localize(TranslationKeys.For("Category", category.Id, "Title"), category.Title),
                    Type = category.Type.ToString().ToLowerInvariant(),
                    IconUrl = category.IconUrl
                })
                .ToList();

            return new DataEnvelope<CategoryResponse>(items);
        }
    }
}
