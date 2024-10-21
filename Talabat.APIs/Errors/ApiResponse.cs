
namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode, string? message = null)
        {
            StatusCode = statusCode;    
            Message = message?? GetDefaultMessageStatusCode(statusCode);
        }

        private string? GetDefaultMessageStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have made",
                401 => "Authorized, you are not",
                404 => "Resource was not found",
                500 => "Errors are the path to the dark side, Errors lead to anger, Anger leads to hate, Hate leads to career change",
                _ => null
            }; 

             //( شغل فلاااااااااحين )
             //if (statusCode == 400)
             //    return "A bad request, you have made";
             //else if (statusCode == 401)
             //    return "Authorized, you are not";
             //else if (statusCode == 404)
             //    return "Resource was not found";
             //else if (statusCode == 500)
             //    return "Errors are the path to the dark side, Errors lead to anger, Anger leads to hate, Hate leads to career change"
            
            
             //else
             //    return "null";
        }
    }
}
