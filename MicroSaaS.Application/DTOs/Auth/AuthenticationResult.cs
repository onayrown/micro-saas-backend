using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.DTOs.Auth;

public class AuthenticationResult
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; }
    public User User { get; private set; }
    public string Token { get; private set; }

    private AuthenticationResult(bool isSuccess, string errorMessage, User user, string token)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        User = user;
        Token = token;
    }

    public static AuthenticationResult CreateSuccess(User user, string token)
    {
        return new AuthenticationResult(true, string.Empty, user, token);
    }

    public static AuthenticationResult CreateFailure(string errorMessage)
    {
        return new AuthenticationResult(false, errorMessage, null, string.Empty);
    }
} 