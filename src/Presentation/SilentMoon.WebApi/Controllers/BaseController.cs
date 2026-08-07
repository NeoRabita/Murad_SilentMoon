using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SilentMoon.Application.DTOs.Storage;
using SilentMoon.WebApi.Services;

namespace SilentMoon.WebApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BaseController() : ControllerBase
    {
        private IProblemResultFactory _problemFactory;
        protected IProblemResultFactory ProblemFactory =>
            _problemFactory ??= HttpContext.RequestServices.GetRequiredService<IProblemResultFactory>();
        private IDispatcher _dispatcher;
        protected IDispatcher Dispatcher =>
            _dispatcher ??= HttpContext.RequestServices.GetRequiredService<IDispatcher>();

        protected IResult HandleResult<T>(Result<T> result) =>
                result.IsSuccess ? Results.Ok(result.Value) : ProblemFactory.CreateProblem(result);

        protected IResult HandleResult(Result result) =>
            result.IsSuccess ? Results.Ok() : ProblemFactory.CreateProblem(result);

        protected IResult HandleStreamResult(Result<ObjectRangeResult> result)
        {
            if (!result.IsSuccess)
            {
                return ProblemFactory.CreateProblem(result);
            }

            var range = result.Value;

            Response.Headers.AcceptRanges = "bytes";

            if (range.IsPartial)
            {
                Response.StatusCode = StatusCodes.Status206PartialContent;
                Response.Headers.ContentRange = $"bytes {range.RangeStart}-{range.RangeEnd}/{range.TotalSize}";
            }

            return Results.Stream(range.Content, range.ContentType ?? "application/octet-stream");
        }
    }
}