namespace Components.Result
{
    public class ErrorDescription
    {
        #region Fields

        private Exception? _exception;

        #endregion

        #region Constructors

        public ErrorDescription(ErrorType errorType)
        {
            Uid = Guid.NewGuid();
            ErrorType = errorType;
            ErrorMessage = errorType.ToString();
        }

        public ErrorDescription(ErrorType errorType, string errorMessage) : this(errorType)
        {
            ErrorMessage = string.IsNullOrEmpty(errorMessage) ? errorType.ToString() : errorMessage;
        }

        public ErrorDescription(ErrorType errorType, string errorMessage, Exception? exception) : this(errorType, errorMessage)
        {
            Exception = exception;
        }

        #endregion

        #region Properties

        public ErrorType ErrorType { get; private set; }

        public string? ErrorMessage { get; set; }

        public string? StackTrace { get; private set; }

        internal Exception? Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                StackTrace = _exception?.StackTrace;
            }
        }

        public Guid Uid { get; private set; }

        #endregion

        public ResultException AsException() => new(ErrorType, ErrorMessage, Exception);
    }

    public class ErrorDescription<TErrorType> where TErrorType : struct
    {
        #region Fields

        private Exception? _exception;

        #endregion

        #region Constructors

        public ErrorDescription(TErrorType errorType)
        {
            Uid = Guid.NewGuid();
            ErrorType = errorType;
        }

        public ErrorDescription(TErrorType errorType, string errorMessage) : this(errorType)
        {
            ErrorMessage = errorMessage;
        }

        public ErrorDescription(TErrorType errorType, string errorMessage, Exception? exception) : this(errorType, errorMessage)
        {
            Exception = exception;
        }

        #endregion

        #region Properties

        public TErrorType ErrorType { get; private set; }

        public string? ErrorMessage { get; set; }

        public string? StackTrace { get; private set; }

        internal Exception? Exception
        {
            get => _exception;
            set
            {
                _exception = value;
                StackTrace = _exception?.StackTrace;
            }
        }

        public Guid Uid { get; private set; }

        #endregion

        public ResultException<TErrorType> AsException() => new(ErrorType, ErrorMessage, Exception);
    }
}
