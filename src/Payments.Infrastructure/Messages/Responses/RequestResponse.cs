namespace Payments.Infrastructure.Messages.Responses
{
    using System;

    public class RequestResponse
    {
        public RequestResponse(bool success)
        {
            Success = success;
            Exception = null;
        }

        public RequestResponse(Exception exception)
        {
            Success = false;
            Exception = exception;
        }

        public bool Success { get; set; }
        public Exception? Exception { get; set; }
    }
}