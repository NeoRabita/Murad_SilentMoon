using System.Collections.Generic;

namespace SilentMoon.WebApi.Contracts;

public class ErrorEnvelope
{
    public ErrorDetail Error { get; set; }
}

public class ErrorDetail
{
    public string Code { get; set; }

    public string Message { get; set; }

    public List<ErrorFieldDetail> Details { get; set; } = new();

    public string RequestId { get; set; }
}

public class ErrorFieldDetail
{
    public string Field { get; set; }

    public string Issue { get; set; }
}
