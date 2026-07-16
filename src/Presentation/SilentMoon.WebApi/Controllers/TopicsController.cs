using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Topics.Queries.GetTopics;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class TopicsController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetTopics()
        {
            var result = await Dispatcher.Send(new GetTopicsQuery());

            return HandleResult(result);
        }
    }
}
