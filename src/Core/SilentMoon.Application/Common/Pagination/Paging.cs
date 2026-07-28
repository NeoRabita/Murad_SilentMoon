using System.Collections.Generic;
using System.Linq;

namespace SilentMoon.Application.Common.Pagination
{
    public static class Paging
    {
        public static List<T> Slice<T>(IEnumerable<T> source, int page, int limit) =>
            source.Skip((page - 1) * limit).Take(limit).ToList();
    }
}
