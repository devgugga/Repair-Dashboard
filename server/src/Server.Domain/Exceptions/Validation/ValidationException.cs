namespace Server.Domain.Exceptions.Validation;

public class DomainValidationException : DomainException
{
    public DomainValidationException(string field, string message) : base("VALIDATION_ERROR", "Validation failed")
    {
        ValidationErrors = new Dictionary<string, string[]> { { field, [message] } };
    }

    public DomainValidationException(Dictionary<string, string[]> validationErrors) : base("VALIDATION_ERROR",
        "Validation failed")
    {
        ValidationErrors = validationErrors;
    }

    public Dictionary<string, string[]> ValidationErrors { get; }
}

public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base("BUSINESS_RULE_VIOLATION", message)
    {
    }

    public BusinessRuleViolationException(string message, Exception innerException) : base("BUSINESS_RULE_VIOLATION",
        message, innerException)
    {
    }
}
