using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace HospitalWeb.Mvc
{
    public class PollyExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<PollyExceptionFilter> _logger;

        public PollyExceptionFilter(ILogger<PollyExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AggregateException aggExc && aggExc.InnerException is TimeoutRejectedException timeoutError)
            {
                _logger.LogError($"Error: {timeoutError.Message}");
                _logger.LogError($"Inner exception:\n{timeoutError.InnerException}");
                _logger.LogTrace(timeoutError.StackTrace);

                context.ExceptionHandled = true;

                context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary()
                        {
                            { "controller", "Error" },
                            { "action", "Timeout" }
                        }
                    );

                return;
            }

            if (context.Exception is AggregateException aggError && aggError.InnerException is BrokenCircuitException cbError)
            {
                _logger.LogError($"Error: {cbError.Message}");
                _logger.LogError($"Inner exception:\n{cbError.InnerException}");
                _logger.LogTrace(cbError.StackTrace);

                context.ExceptionHandled = true;

                context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary()
                        {
                            { "controller", "Error" },
                            { "action", "BrokenCircuit" }
                        }
                    );

                return;
            }
        }
    }
}
