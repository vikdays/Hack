using System.Data;
using System.Net;
using System.Security;
using System.Text.Json;
#pragma warning disable 1591

namespace UserService.Middlewares;

// Обработчик ошибок
public class ExceptionsMiddleware : IMiddleware
{
    private readonly ILogger _logger;

    public ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        
        catch (KeyNotFoundException e)
        {
            _logger.LogError(e, e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await SetProblem(context, e, (int)HttpStatusCode.NotFound);
        }
        
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError(e, e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        
        catch (SecurityException e)
        {
            _logger.LogError(e, e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await SetProblem(context, e, (int)HttpStatusCode.Forbidden);
        }
        
        catch (ConstraintException e)
        {
            _logger.LogError(e, e.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await SetProblem(context, e, (int)HttpStatusCode.BadRequest);
        }
    }
    
    private async Task SetProblem(HttpContext context, Exception e, int status)
    {
        var problem = new
        {
            Status = status,
            Message = e.Message
        };

        string json = JsonSerializer.Serialize(problem);
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(json); 
    }
}