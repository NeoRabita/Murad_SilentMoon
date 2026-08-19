using Application.Abstractions.Messaging;
using SilentMoon.Application.Common;

namespace SilentMoon.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery : IQuery<DataEnvelope<CategoryResponse>>
    {
        public string Type { get; set; }
    }
}
