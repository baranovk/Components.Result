namespace Components.Result
{
    public enum ErrorType
    {
        // User related
        NotFound,
        WrongArguments,
        NotValid,

        // Auth
        NoAuthentication,
        NotAuthorized,

        // Logic related
        Unknown,
        ConfigurationError,
        NetworkError,
        Timeout,
        BadGateway,
        GatewayTimeout,

        Domain
    }
}