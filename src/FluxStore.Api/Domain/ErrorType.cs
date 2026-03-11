namespace FluxStore.Api.Domain
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        IdentityError,
        AccessDenied,
        Failure,
        ConditionNotMet
    }
}