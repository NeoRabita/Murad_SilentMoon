using System.Collections.Generic;

namespace SilentMoon.Application.Common.Pagination
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; }

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int Limit { get; set; }
    }
}
