namespace DTC.API.ErrorHandlers
{
    public class HttpExeption : Exception
    {
        public int StatusCode { get; private set; }

        public HttpExeption(int statusCode, string message) : base($"{statusCode} - {message}")
        {
            StatusCode = statusCode;
        }
    }
}
