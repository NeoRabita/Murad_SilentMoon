using System.Collections.Generic;

namespace SilentMoon.Application.Features.User.Queries.GetHome
{
    public class HomeResponse
    {
        public string Greeting { get; set; }

        public string UserName { get; set; }

        public List<ContentResponse> Featured { get; set; }

        public ContentResponse DailyThought { get; set; }

        public List<ContentResponse> Recommended { get; set; }
    }
}
