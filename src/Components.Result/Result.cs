namespace Components.Result
{
    public class Result
    {
        #region Constants

        protected const string ErrorMessagesDelimeter = "\n";

        #endregion

        #region Fields

        private Dictionary<PropertyType, List<object>>? _propertyBag;
        private ErrorDescription? _errorDescription;

        #endregion

        #region Constructors

        /// <summary>
        /// Positive result. Conclusion is true and result type is Ok.
        /// </summary>
        public Result()
        {
            ResultType = Components.Result.ResultType.Ok;
        }

        /// <summary>
        /// Positive result. Conclusion is true and result type is given as argument.
        /// </summary>
        /// <param name="resultType"></param>
        public Result(ResultType resultType)
        {
            ResultType = resultType;
        }

        /// <summary>
        /// Negative result. Conclusion is false.
        /// </summary>
        /// <param name="errorType"></param>
        public Result(ErrorType errorType)
        {
            ErrorDescription = new ErrorDescription(errorType);
        }

        public Result(ErrorType errorType, Exception exception)
        {
            ErrorDescription = new ErrorDescription(errorType, exception.Message, exception);
        }

        public Result(ErrorType errorType, string errorMessage, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription(errorType, errorMessage, exception);
        }

        public Result(ErrorDescription errorDescription)
        {
            ErrorDescription = errorDescription;
        }

        public Result(ErrorType errorType, IEnumerable<string> errorMessages, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription(errorType, string.Join(ErrorMessagesDelimeter, errorMessages), exception);
        }

        #endregion

        #region Properties

        public static Result Success => new();

        /// <summary>
        /// ResultType of the method.
        /// </summary>
        public ResultType? ResultType { get; protected set; }

        /// <summary>
        /// Description of the error. Can be null.
        /// </summary>
        public ErrorDescription? ErrorDescription
        {
            get => _errorDescription;

            private set
            {
                _errorDescription = value;
                ResultType = null;
            }
        }

        public bool IsSuccess => ResultType is not null && ErrorDescription is null;

        public bool IsError => ResultType is null && ErrorDescription is not null;

        #endregion

        #region Public Methods

        public Result EnsureSuccess()
        {
            return null != ErrorDescription ? throw ErrorDescription!.AsException() : this;
        }

        public Result SetError(ErrorType errorType)
        {
            ErrorDescription = new ErrorDescription(errorType);
            return this;
        }

        public Result SetError(ErrorType errorType, IEnumerable<string> errorMessages, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription(errorType, string.Join(ErrorMessagesDelimeter, errorMessages), exception);
            return this;
        }

        public Result SetError(ErrorType errorType, string errorMessage, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription(errorType, errorMessage, exception);
            return this;
        }

        public Result SetProperty(PropertyType type, object value)
        {
            if (null == _propertyBag)
            {
                _propertyBag = new();
            }

            if (!_propertyBag.TryGetValue(type, out var values))
            {
                _propertyBag.Add(type, new List<object> { value });
            }
            else
            {
                values.Add(value);
            }

            return this;
        }

        public bool HasPropertiesOf(PropertyType type) => null != _propertyBag && _propertyBag.ContainsKey(type);

        public IEnumerable<object>? GetProperties(PropertyType type) => HasPropertiesOf(type) ? _propertyBag![type] : null;

        public bool IsErrorOfType(ErrorType type) => null != ErrorDescription && ErrorDescription.ErrorType.Equals(type);

        #endregion
    }

    [ResultOutput]
    public class Result<TOutput> : Result
    {
        #region Constructors

        /// <summary>
        /// The desired output.
        /// </summary>  
        public Result() : base() { }

        public Result(ResultType resultType) : base(resultType) { }

        public Result(ResultType resultType, TOutput? value) : base(resultType) { SetOutput(value); }

        public Result(ErrorType errorType) : base(errorType) { }

        public Result(ErrorType errorType, Exception exception) : base(errorType, exception) { }

        public Result(ErrorType errorType, string errorMessage, Exception? exception = null) : base(errorType, errorMessage, exception) { }

        public Result(ErrorDescription errorDescription) : base(errorDescription) { }

        #endregion

        #region Properties

        public TOutput? Output { get; protected set; }

        #endregion

        #region Public Methods

        public Result<TOutput> AddMethodInfo(params string[] infos)
        {
            if (null != ErrorDescription)
            {
                // Prepend error message with additional info
                ErrorDescription.ErrorMessage = $"{typeof(TOutput).Name} {string.Join(", ", infos)}, {ErrorDescription.ErrorMessage}";
            }

            return this;
        }

        public Result<TOutput> SetOutput(TOutput? value)
        {
            Output = value;
            return this;
        }

        /// <summary>
        /// Makes sure that Result is positive and the Output is not null. Otherwise Exception is thrown.
        /// Not null Output is returned.
        /// </summary>
        /// <returns></returns>
        public TOutput EnsureOutput()
        {
            if (IsSuccess && (typeof(TOutput).IsValueType || !Equals(Output, default(TOutput))))
            {
                return Output!;
            }

            if (IsError)
            {
                throw ErrorDescription!.AsException();
            }

            throw new InvalidOperationException(nameof(EnsureOutput));
        }

        #endregion
    }

    [ResultOutput]
    public class Result<TOutput, TErrorType> : Result<TOutput> where TErrorType : struct
    {
        private ErrorDescription<TErrorType>? _errorDescription;

        #region Constructors

        public Result() : base() { }

        public Result(ResultType resultType) : base(resultType) { }

        public Result(ResultType resultType, TOutput? value) : base(resultType, value) { }

        public Result(TErrorType errorType)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType);
        }

        public Result(TErrorType errorType, Exception exception)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType, exception.Message, exception);
        }

        public Result(TErrorType errorType, string errorMessage, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType, errorMessage, exception);
        }

        public Result(ErrorDescription<TErrorType> errorDescription)
        {
            ErrorDescription = errorDescription;
        }

        public Result(TErrorType errorType, IEnumerable<string> errorMessages, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType, string.Join(ErrorMessagesDelimeter, errorMessages), exception);
        }

        #endregion

        public new ErrorDescription<TErrorType>? ErrorDescription
        {
            get => _errorDescription;

            private set
            {
                _errorDescription = value;
                ResultType = null;
            }
        }

        public new bool IsSuccess => null != ResultType && ErrorDescription is null;

        public new bool IsError => null == ResultType && null != ErrorDescription;

        public new Result<TOutput, TErrorType> SetOutput(TOutput value)
        {
            Output = value;
            return this;
        }

        public Result<TOutput, TErrorType> SetError(TErrorType errorType)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType);
            return this;
        }

        public Result<TOutput, TErrorType> SetError(TErrorType errorType, IEnumerable<string> errorMessages, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType, string.Join(ErrorMessagesDelimeter, errorMessages), exception);
            return this;
        }

        public Result<TOutput, TErrorType> SetError(TErrorType errorType, string errorMessage, Exception? exception = null)
        {
            ErrorDescription = new ErrorDescription<TErrorType>(errorType, errorMessage, exception);
            return this;
        }

        public bool IsErrorOfType(TErrorType type) => null != ErrorDescription && ErrorDescription.ErrorType.Equals(type);
    }
}
