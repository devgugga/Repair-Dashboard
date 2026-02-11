namespace Server.Domain.Exceptions;

public class DomainException : Exception
{
    protected DomainException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    protected DomainException(string errorCode, string message, Exception innerException) : base(message,
        innerException)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}
