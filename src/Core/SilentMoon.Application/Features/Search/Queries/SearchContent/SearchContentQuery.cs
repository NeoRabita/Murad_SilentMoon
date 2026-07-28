using Application.Abstractions.Messaging;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.Search.Queries.SearchContent
{
    public class SearchContentQuery : IQuery<List<SearchResultItemResponse>>
    {
        public string Term { get; set; }
    }
}
