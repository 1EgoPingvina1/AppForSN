namespace DTC.Application.ErrorHandlers
{
    public class HttpExeption : Exception
    {
        public int StatusCode { get; private set; }

        public HttpExeption(int statusCode, string message) : base($"{message}")
        {
            StatusCode = statusCode;
        }
    }
}
