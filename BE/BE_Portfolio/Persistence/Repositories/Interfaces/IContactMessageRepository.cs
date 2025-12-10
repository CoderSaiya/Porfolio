using BE_Portfolio.Models.Domain;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IContactMessageRepository
{
    Task InsertAsync(ContactMessage doc, CancellationToken ct = default);
    Task<List<ContactMessage>> ListAsync(MessageFilter filter, CancellationToken ct = default);
    Task UpdateStatusAsync(string id, MessageStatus status, CancellationToken ct = default);
    Task<ContactMessage?> GetByIdAsync(string id, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task BulkDeleteAsync(List<string> ids, CancellationToken ct = default);
    Task<long> CountAsync(MessageFilter filter, CancellationToken ct = default);
}