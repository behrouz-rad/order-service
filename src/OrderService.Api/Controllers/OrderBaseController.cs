using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Api.Controllers;

[Controller]
[ApiController]
[Route("api/[controller]")]
public abstract class OrderBaseController : ControllerBase
{
    protected IActionResult BadRequest(ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Result must not be successful to convert to ProblemDetails.");
        }

        return new ObjectResult(new ProblemDetails
        {
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", result.Errors.Select(e => e.Message).ToArray() }
            }
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }

    protected IActionResult NotFound(ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Result must not be successful to convert to ProblemDetails.");
        }

        return new ObjectResult(new ProblemDetails
        {
            Title = "Not Found",
            Status = StatusCodes.Status404NotFound,
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", result.Errors.Select(e => e.Message).ToArray() }
            }
        })
        {
            StatusCode = StatusCodes.Status404NotFound
        };
    }

    protected IActionResult InternalServerError(ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Result must not be successful to convert to ProblemDetails.");
        }

        return new ObjectResult(new ProblemDetails
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", result.Errors.Select(e => e.Message).ToArray() }
            }
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
