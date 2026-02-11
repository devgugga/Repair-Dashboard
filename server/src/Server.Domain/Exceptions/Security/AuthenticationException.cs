namespace Server.Domain.Exceptions.Security;

public class AuthenticationException : DomainException
{
    public AuthenticationException(string message) : base("AUTH_FAILED", message)
    {
    }

    public AuthenticationException(string message, Exception innerException) : base("AUTH_FAILED", message,
        innerException)
    {
    }
}

public class InvalidCredentialsException() : AuthenticationException("Invalid username or password");

public class InvalidTokenException(string message = "Invalid or expired token") : AuthenticationException(message);

public class TokenExpiredException() : AuthenticationException("Token has expired");
