namespace InvenTrack.Application.Common.Interfaces;

using System.Threading;
using System.Threading.Tasks;

public interface IUnitOfWork
{
    ICategoryRepository Categories { get; }
    IProductRepository Products { get; }
    ISupplierRepository Suppliers { get; }
    ISaleRepository Sales { get; }
    IPurchaseRepository Purchases { get; }
    IInventoryRepository Inventory { get; }
    IDashboardRepository Dashboard { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
