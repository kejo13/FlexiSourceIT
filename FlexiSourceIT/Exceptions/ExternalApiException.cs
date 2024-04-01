using FlexiSourceIT.Models;

namespace FlexiSourceIT.Exceptions
{
    public class ExternalApiException : Exception
    {
        public ErrorResponse _errorDetail { get; }
        public ExternalApiException(string message, ErrorResponse errorDetail)
            : base(message)
        {
            _errorDetail = errorDetail;
        }
        public ExternalApiException(string message, ErrorResponse errorDetail, Exception innerException)
            : base(message, innerException)
        {
            _errorDetail = errorDetail;
        }
    }
}
