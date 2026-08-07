namespace InvenTrack.Infrastructure.Persistence;

using InvenTrack.Application.Common.Interfaces;
using InvenTrack.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private ICategoryRepository? _categories;
    private IProductRepository? _products;
    private ISupplierRepository? _suppliers;
    private ISaleRepository? _sales;
    private IPurchaseRepository? _purchases;
    private IInventoryRepository? _inventory;
    private IDashboardRepository? _dashboard;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public ISupplierRepository Suppliers => _suppliers ??= new SupplierRepository(_context);
    public ISaleRepository Sales => _sales ??= new SaleRepository(_context);
    public IPurchaseRepository Purchases => _purchases ??= new PurchaseRepository(_context);
    public IInventoryRepository Inventory => _inventory ??= new InventoryRepository(_context);
    public IDashboardRepository Dashboard => _dashboard ??= new DashboardRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
