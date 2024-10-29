namespace ActivityGameBackend.Application.Helpers;

public static class GuardClauses
{
    public static T EnsureNotNull<T>(this T? value, Func<Exception> exceptionFactory) where T : class
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        return value ?? throw exceptionFactory();
    }
}