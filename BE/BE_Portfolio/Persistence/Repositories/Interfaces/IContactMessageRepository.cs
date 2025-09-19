﻿using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IContactMessageRepository
{
    Task InsertAsync(ContactMessage doc, CancellationToken ct = default);
    Task<List<ContactMessage>> ListAsync(MessageStatus? status, int? limit, CancellationToken ct = default);
    Task UpdateStatusAsync(string id, MessageStatus status, CancellationToken ct = default);
}