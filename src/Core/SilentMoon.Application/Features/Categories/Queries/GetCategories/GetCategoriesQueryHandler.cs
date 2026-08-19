using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Domain.Enums;
using SilentMoon.SharedKernel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public GetCategoriesQueryHandler(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
        }

        public Task<Result<List<CategoryResponse>>> Handle(GetCategoriesQuery query, CancellationToken ct)
        {
            List<CategoryResponse> categories = Enum.GetValues<ContentCategory>()
                .Select(category => new CategoryResponse
                {
                    Id = category.ToString().ToLowerInvariant(),
                    Name = _localizer.LocalizeCategory(category)
                })
                .ToList();

            Result<List<CategoryResponse>> result = categories;

            return Task.FromResult(result);
        }
    }
}
