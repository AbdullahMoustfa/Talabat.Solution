namespace Talabat.APIs.Errors
{
    public class ApiExceptionMiddleWareResponse : ApiResponse
    {
        public string? Details { get; set; }
        public ApiExceptionMiddleWareResponse(int statusCode,string? message = null,string? details = null)
               : base (statusCode,message)
        {
            Details = details;
        }


    }
}
