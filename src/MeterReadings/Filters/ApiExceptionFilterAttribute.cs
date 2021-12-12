using MeterReadings.Controllers;
using MeterReadings.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeterReadings
{
    /// <summary>
    /// Annotate a declaration of a controller or controller route to catch standard exceptions and alter the HTTP response as appropriate.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public ApiExceptionFilterAttribute() { }

        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case EntityNotFoundException:
                    context.Result = new NotFoundResult();
                    break;
                case Exception:
                    context.Result = new JsonResult(new UnsuccessfulApiResponse(context.Exception));
                    break;
            }
        }
    }
}