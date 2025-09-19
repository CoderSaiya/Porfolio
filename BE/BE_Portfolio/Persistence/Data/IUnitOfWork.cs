using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    IClientSessionHandle? Session { get; }
    Task BeginAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task AbortAsync(CancellationToken ct = default);
}