using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Courses.Queries.GetCourseDetails;
using SilentMoon.Application.Features.Courses.Queries.GetCourses;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
    [Authorize]
    public class CoursesController : BaseController
    {
        [HttpGet]
        public async Task<IResult> GetCourses([FromQuery] GetCoursesQuery query)
        {
            var result = await Dispatcher.Send(query);

            return HandleResult(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IResult> GetCourseDetails(int id)
        {
            var result = await Dispatcher.Send(new GetCourseDetailsQuery
            {
                ContentId = id
            });

            return HandleResult(result);
        }
    }
}
