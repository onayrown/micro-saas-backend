namespace MicroSaaS.Shared.Exceptions;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message)
    {
    }
} 