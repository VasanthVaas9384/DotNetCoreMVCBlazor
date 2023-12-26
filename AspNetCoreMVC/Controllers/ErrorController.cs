using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreMVC.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodePages(int statusCode)
        {
            var statusCodeRes = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            if (statusCodeRes != null)
            {
                switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMEssage = "Sorry, the requested resource not found";
                    ViewBag.ErrorPath = statusCodeRes.OriginalPath;
                    ViewBag.QueryString = statusCodeRes.OriginalQueryString;
                    ViewBag.ErrorMEssage = "Sorry, the requested resource not found";
                    logger.LogError($"{ViewBag.ErrorMEssage} /   /  {ViewBag.ErrorPath}");

                    break;
            }
            return View("../CustomError/CentralisedError");
            }
            else
                return RedirectToAction("Error");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionDetails != null)
            {

                ViewBag.ExceptionPath = exceptionDetails.Path;
                ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
                ViewBag.StackTrace = exceptionDetails.Error.StackTrace;

                logger.LogError($"{exceptionDetails.Path} /   /  {exceptionDetails.Error.Message}");

                return View("../CustomError/GlobalException");
            }
            return null;
        }
    }
}
