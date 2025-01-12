namespace Components.Result
{
    public class ResultException : Exception
    {
        public ResultException(ErrorType errorType) : base(errorType.ToString())
        {
            ErrorType = errorType;
        }

        public ResultException(ErrorType errorType, string? errorMessage, Exception? exception) : base($"{errorType}: {errorMessage}", exception)
        {
            ErrorType = errorType;
            ErrorMessageInitial = errorMessage;
        }

        public ErrorType ErrorType { get; private set; }

        public string? ErrorMessageInitial { get; private set; }

        public Result AsResult()
        {
            return new Result(ErrorType, Message, InnerException);
        }
    }

    public class ResultException<TErrorType> : Exception where TErrorType : struct
    {
        public ResultException(TErrorType errorType) : base(errorType.ToString())
        {
            ErrorType = errorType;
        }

        public ResultException(TErrorType errorType, string? errorMessage, Exception? exception) : base($"{errorType}: {errorMessage}", exception)
        {
            ErrorType = errorType;
            ErrorMessageInitial = errorMessage;
        }

        public TErrorType ErrorType { get; private set; }

        public string? ErrorMessageInitial { get; private set; }
    }
}