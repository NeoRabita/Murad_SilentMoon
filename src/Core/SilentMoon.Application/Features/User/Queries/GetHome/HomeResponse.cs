namespace SilentMoon.Application.Features.User.Queries.GetHome
{
    public class HomeResponse
    {
        public HomeSectionResponse Recommended { get; set; }

        public HomeItemResponse DailyThought { get; set; }

        public HomeSectionResponse FeaturedSleep { get; set; }

        public HomeSectionResponse PopularMeditations { get; set; }
    }
}
