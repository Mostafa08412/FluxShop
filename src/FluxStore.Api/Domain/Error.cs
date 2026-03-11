namespace FluxStore.Api.Domain
{
    public record Error
    {
        public string Code { get; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ErrorType ErrorType { get; }

        public string Identifier => Code.Split("__").ElementAtOrDefault(1) ?? "";

        public Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }
    }
}
