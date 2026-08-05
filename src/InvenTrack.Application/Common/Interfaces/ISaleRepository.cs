namespace InvenTrack.Application.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InvenTrack.Domain.Entities;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sale>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default);
}
