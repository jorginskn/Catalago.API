using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalago.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger _logger;

        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            //Antes do metodo action
            _logger.LogInformation("### EXECUTADO -> OnActionExecuting");
            _logger.LogInformation("##################################");
            _logger.LogInformation($"{DateTime.Now.ToLongDateString()}");
            _logger.LogInformation($"Status code :{context.HttpContext.Response.StatusCode}");

    
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            //Apos o metodo action
            _logger.LogInformation("##################################");
            _logger.LogInformation("### EXECUTADO -> OnActionExecuted");
            _logger.LogInformation("##################################");
            _logger.LogInformation($"{DateTime.Now.ToLongDateString()}");
            _logger.LogInformation($"ModelState :{context.ModelState.IsValid}");
            _logger.LogInformation("##################################");
        }
    }
}
