namespace FluxStore.Api.Shared.Abstractions
{
    public interface IEmailModel
    {
        string TemplateName { get; }

        string Subject { get; }
    }
}
