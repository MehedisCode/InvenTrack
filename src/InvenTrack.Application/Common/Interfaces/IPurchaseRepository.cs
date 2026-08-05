namespace InvenTrack.Application.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Domain.Entities;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Purchase> AddAsync(Purchase purchase, CancellationToken cancellationToken = default);
}
