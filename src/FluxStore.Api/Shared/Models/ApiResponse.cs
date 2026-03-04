namespace FluxStore.Api.Shared.Models
{
    public record ApiResponse
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public string ErrorCode { get; private set; }
        public int StatusCode { get; private set; }
        public IDictionary<string, string> ValidationErrors { get; private set; } = new Dictionary<string, string>();
        public IDictionary<string, string> Meta { get; private set; } = new Dictionary<string, string>();
        public string Instance { get; set; }
        public string TraceId { get; set; }

        public ApiResponse(bool isSuccess, string message, string errorCode, int statusCode, IDictionary<string, string> validationErrors, IDictionary<string, string> meta, HttpContext context)
        {
            IsSuccess = isSuccess;
            Message = message;
            ErrorCode = errorCode;
            StatusCode = statusCode;
            ValidationErrors = validationErrors;
            Meta = meta;
            Instance = context.Request.Path.Value!;
            TraceId = context.TraceIdentifier;
        }
        public static ApiResponse Failure(HttpContext context, string message, string errorCode = "VALIDATION_ERROR", int statusCode = 400, IDictionary<string, string> validationErrors = null) =>
        new(false, message, errorCode, statusCode, validationErrors?.ToDictionary(keySelector => keySelector.Key.ToLowerInvariant(), keySelector => keySelector.Value) ?? new Dictionary<string, string>(), new Dictionary<string, string>(), context);
        public static ApiResponse Success(HttpContext context, string message = "Operation successful", int statusCode = 200, IDictionary<string, string> meta = null) =>
        new(true, message, string.Empty, statusCode, new Dictionary<string, string>(), meta, context);
    }
    public record ApiResponse<T> : ApiResponse
    {
        public ApiResponse(T data, bool isSuccess, string message, string errorCode, int statusCode, IDictionary<string, string> validationErrors, IDictionary<string, string> meta, HttpContext context)
        : base(isSuccess, message, errorCode, statusCode, validationErrors, meta ?? new Dictionary<string, string>(), context)
        {
            Data = data;
        }

        public T Data { get; private set; }

        public static ApiResponse<T> Success<T>(HttpContext context, T data, string message = "Operation successful", int statusCode = 200, IDictionary<string, string> meta = null) =>
         new(data, true, message, string.Empty, statusCode, new Dictionary<string, string>(), meta ?? new Dictionary<string, string>(), context);


    }

}
