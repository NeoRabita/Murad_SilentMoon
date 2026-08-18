using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SilentMoon.Application.DTOs.Storage;
using SilentMoon.WebApi.Services;
using System.IO;
using System.Threading.Tasks;

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

        protected IActionResult HandleStreamResult(Result<ObjectRangeResult> result)
        {
            if (!result.IsSuccess)
            {
                // ProblemFactory IResult qaytarır — burada IActionResult lazımdır
                // Ona görə HandleResult-dən istifadə edib StatusCode ilə wrap edirik
                var statusCode = result.Error.Type switch
                {
                    ErrorType.NotFound    => StatusCodes.Status404NotFound,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorType.Validation  => StatusCodes.Status400BadRequest,
                    _                    => StatusCodes.Status500InternalServerError
                };
                return StatusCode(statusCode, new { errorCode = result.Error.Code, detail = result.Error.Description });
            }

            var range = result.Value;
            var contentType = range.ContentType ?? "audio/mpeg";

            // Browser-ın seek edə bilməsi üçün həmişə lazımdır
            Response.Headers.AcceptRanges = "bytes";

            if (range.IsPartial)
            {
                // Content-Range: bytes start-end/total
                Response.Headers.ContentRange =
                    $"bytes {range.RangeStart}-{range.RangeEnd}/{range.TotalSize}";

                // Bu chunk-ın byte ölçüsü
                Response.Headers.ContentLength = range.RangeEnd - range.RangeStart + 1;

                // PartialFileStreamResult → ExecuteResultAsync-da 206 set edir
                return new PartialFileStreamResult(range.Content, contentType);
            }

            // Range header yoxdursa — tam faylı 200 ilə qaytar
            Response.Headers.ContentLength = range.TotalSize;

            return new FileStreamResult(range.Content, contentType)
            {
                EnableRangeProcessing = false
            };
        }

        /// <summary>
        /// FileStreamResult-in ExecuteResultAsync metodunu override edərək
        /// status kodunu 206-ya set edir.
        /// OnActionExecuted override mümkün deyil (primary constructor BaseController).
        /// </summary>
        private sealed class PartialFileStreamResult : FileStreamResult
        {
            public PartialFileStreamResult(Stream stream, string contentType)
                : base(stream, contentType)
            {
                EnableRangeProcessing = false;
            }

            public override async Task ExecuteResultAsync(ActionContext context)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status206PartialContent;
                await base.ExecuteResultAsync(context);
            }
        }
    }
}