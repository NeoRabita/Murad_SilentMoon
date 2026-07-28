namespace SilentMoon.Application.Common.Pagination
{
    public class PagedQuery
    {
        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 20;

        public int NormalizedPage => Page < 1 ? 1 : Page;

        public int NormalizedLimit => Limit < 1 ? 20 : Limit;
    }
}
