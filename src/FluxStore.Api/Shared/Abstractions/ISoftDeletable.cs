namespace FluxStore.Api.Shared.Abstractions;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUTC { get; }

    void SoftDelete();
    void Restore();
}
