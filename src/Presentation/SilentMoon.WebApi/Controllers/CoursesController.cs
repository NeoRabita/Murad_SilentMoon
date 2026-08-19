using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SilentMoon.Application.Features.Courses.Queries.GetCourseDetails;
using SilentMoon.Application.Features.Courses.Queries.GetCourseFullDetail;
using SilentMoon.Application.Features.Courses.Queries.GetCourses;
using SilentMoon.Application.Features.Courses.Queries.GetCourseTracks;
using SilentMoon.Application.Features.Courses.Queries.GetRelatedCourses;
using System.Threading.Tasks;

namespace SilentMoon.WebApi.Controllers
{
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
            var result = await Dispatcher.Send(new GetCourseDetailsQuery { ContentId = id });

            return HandleResult(result);
        }

        [HttpGet("{id:int}/tracks")]
        public async Task<IResult> GetCourseTracks(int id, [FromQuery] string narrator)
        {
            var result = await Dispatcher.Send(new GetCourseTracksQuery { ContentId = id, Narrator = narrator });

            return HandleResult(result);
        }

        [HttpGet("{id:int}/detail")]
        public async Task<IResult> GetCourseFullDetail(int id)
        {
            var result = await Dispatcher.Send(new GetCourseFullDetailQuery { ContentId = id });

            return HandleResult(result);
        }

        [HttpGet("{id:int}/related")]
        public async Task<IResult> GetRelatedCourses(int id, [FromQuery] int? limit)
        {
            var result = await Dispatcher.Send(new GetRelatedCoursesQuery { ContentId = id, Limit = limit });

            return HandleResult(result);
        }
    }
}
