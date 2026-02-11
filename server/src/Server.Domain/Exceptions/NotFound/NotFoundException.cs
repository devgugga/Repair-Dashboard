namespace Server.Domain.Exceptions.NotFound;

public class NotFoundException : DomainException
{
    public NotFoundException(string resource) : base("NOT_FOUND", $"{resource} not found")
    {
    }

    public NotFoundException(string resource, string identifier) : base("NOT_FOUND",
        $"{resource} with identifier '{identifier}' was not found")
    {
    }
}

public class UserNotFoundException(string identifier) : NotFoundException("User", identifier);

public class RefreshTokenNotFoundException() : NotFoundException("Refresh token");
