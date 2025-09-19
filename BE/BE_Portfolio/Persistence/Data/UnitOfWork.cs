using BE_Portfolio.Models.Commons;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Data;

public sealed class UnitOfWork(IOptions<MongoDbSettings> settings) : IUnitOfWork
{
    private readonly IMongoClient _client = new MongoClient(settings.Value.ConnectionString);
    public IClientSessionHandle? Session { get; private set; }

    public async Task BeginAsync(CancellationToken ct = default)
    {
        if (Session is not null) return;
        Session = await _client.StartSessionAsync(cancellationToken: ct);
        Session.StartTransaction(); // yêu cầu replica set để dùng transaction
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (Session is null) return;
        if (Session.IsInTransaction) await Session.CommitTransactionAsync(ct);
        Session.Dispose();
        Session = null;
    }

    public async Task AbortAsync(CancellationToken ct = default)
    {
        if (Session is null) return;
        if (Session.IsInTransaction) await Session.AbortTransactionAsync(ct);
        Session.Dispose();
        Session = null;
    }

    public ValueTask DisposeAsync()
    {
        Session?.Dispose();
        return ValueTask.CompletedTask;
    }
}