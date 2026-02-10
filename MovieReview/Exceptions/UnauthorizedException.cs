namespace MovieReview.Exceptions;

/// <summary>
/// Thrown when credentials are invalid (e.g. login failure). Middleware maps to 401.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
