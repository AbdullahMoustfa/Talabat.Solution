using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    // By Convension 

    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IWebHostEnvironment env)
        {
           _logger = logger;
           _next = next;
           _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //Take an Action With Request


                await _next.Invoke(httpContext);


                //Take an Action With Response
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message); // Exception in Development
                // Exception in Production (Database | Files)
            
               httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
               httpContext.Response.ContentType = "application/json";

                var response = _env.IsDevelopment() ?
                               new ApiExceptionMiddleWareResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace.ToString())
                               :
                               new ApiExceptionMiddleWareResponse((int)HttpStatusCode.InternalServerError);
            
                var options = new JsonSerializerOptions{ PropertyNamingPolicy =  JsonNamingPolicy.CamelCase };

               var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);
            }
        }


    }
}
