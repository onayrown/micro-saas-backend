using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Services;

public class AuthenticationResult
{
    public bool IsSuccess { get; private set; }
    public User User { get; private set; }
    public string Token { get; private set; }
    public string ErrorMessage { get; private set; }

    private AuthenticationResult() { }

    public static AuthenticationResult Success(User user, string token)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            User = user,
            Token = token
        };
    }

    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
