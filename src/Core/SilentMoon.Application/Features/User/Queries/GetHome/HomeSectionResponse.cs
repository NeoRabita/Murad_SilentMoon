using System.Collections.Generic;

namespace SilentMoon.Application.Features.User.Queries.GetHome
{
    public class HomeSectionResponse
    {
        public string Title { get; set; }

        public List<HomeItemResponse> Items { get; set; }
    }
}
