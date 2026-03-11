namespace LMS.Domain.Exceptions;

/// <summary>Base class for all domain-specific exceptions.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

/// <summary>Thrown when a requested resource does not exist.</summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with identifier '{key}' was not found.") { }
}

/// <summary>Thrown when a business rule is violated.</summary>
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Thrown when the caller is not authorized to perform the action.</summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Access denied.") : base(message) { }
}

/// <summary>Thrown when input data fails domain validation.</summary>
public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message) { }
}
