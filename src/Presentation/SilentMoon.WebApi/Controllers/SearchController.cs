using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Search.Queries.SearchContent;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    [Authorize]
    public class SearchController : BaseController
    {
        [HttpGet]
        public async Task<IResult> Search([FromQuery] string q)
        {
            var result = await Dispatcher.Send(new SearchContentQuery
            {
                Term = q
            });

            return HandleResult(result);
        }
    }
}
