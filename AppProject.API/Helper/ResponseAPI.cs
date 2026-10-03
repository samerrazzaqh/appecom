namespace AppProject.API.Helper
{
    public class ResponseAPI
    {
        public ResponseAPI(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetMessageFromStatusCode(StatusCode);
        }

        private string? GetMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Done",
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Resource not found",
                500 => "Server Error",
                _ => null,
            };
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }

    public class ResponseAPI<T> : ResponseAPI
    {
        public T? Data { get; set; }

        public ResponseAPI(int statusCode, T? data, string? message = null) 
            : base(statusCode, message)
        {
            Data = data;
        }
    }
    
    
    

}
