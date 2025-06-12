using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Domain.Abstractions;

namespace WebAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (CustomValidationException customException)
            {
                var response = Response.Failure(customException.Errors!.ToList()!);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Ocurrió una excepción: {ex.Message}");

                var exceptionDetails = GetExceptionDetails(ex);

                var problemDetails = new ProblemDetails
                {
                    Status = exceptionDetails.status,
                    Type = exceptionDetails.type,
                    Title = exceptionDetails.title,
                    Detail = exceptionDetails.detail,
                };

                if (exceptionDetails.errors is not null)
                {
                    problemDetails.Extensions["errors"] = exceptionDetails.errors;
                }

                context.Response.StatusCode = exceptionDetails.status;

                await context.Response.WriteAsJsonAsync(Response.Failure(Error.Custom("GENERIC_ERROR",problemDetails.Detail)));
            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception exception)
        {
            return exception switch
            {
                CustomValidationException validationException => new ExceptionDetails(
                    StatusCodes.Status400BadRequest,
                    "ValidationFailure",
                    "Validación de Error",
                    "Han ocurrido uno o mas errores de validacion",
                    validationException.Errors),
                _ => new ExceptionDetails(
                    StatusCodes.Status500InternalServerError,
                    "ServerError",
                    "Error de Servidor",
                    "Un inesperado error ocurrió en la App",
                    null)
            };
        }

        internal record ExceptionDetails(
            int status,
            string type,
            string title,
            string detail,
            IEnumerable<object>? errors);
    }
}
