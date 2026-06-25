using Microsoft.AspNetCore.Http;

namespace SilentMoon.WebApi.Services;

public interface IProblemResultFactory
{
    public IResult CreateProblem(Result result);
}
