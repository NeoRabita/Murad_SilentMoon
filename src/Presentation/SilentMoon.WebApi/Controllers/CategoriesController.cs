using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Categories.Queries.GetCategories;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    public class CategoriesController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetCategories([FromQuery] string type)
        {
            var result = await Dispatcher.Send(new GetCategoriesQuery { Type = type });

            return HandleResult(result);
        }
    }
}
