namespace InvenTrack.Infrastructure.Persistence;

using InvenTrack.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using InvenTrack.Application.Common.Interfaces;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>, IApplicationDbContext
{
    private IDatabaseFacade? _databaseFacade;
    public new IDatabaseFacade Database => _databaseFacade ??= new DatabaseFacadeWrapper(this);

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.SKU).IsUnique();
            entity.Property(p => p.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Suppliers)
                .WithMany(s => s.Products)
                .UsingEntity(j => j.ToTable("SupplierProducts"));
        });

        builder.Entity<StockTransaction>(entity =>
        {
            entity.HasOne(st => st.Product)
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(st => st.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(st => st.User)
                .WithMany(u => u.StockTransactions)
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(st => st.Sale)
                .WithMany(s => s.StockTransactions)
                .HasForeignKey(st => st.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(st => st.Purchase)
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(st => st.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<User>(entity =>
        {
            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Sale>(entity =>
        {
            entity.HasIndex(s => s.SaleNumber).IsUnique();
            entity.Property(s => s.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(s => s.User)
                .WithMany(u => u.Sales)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SaleItem>(entity =>
        {
            entity.Property(si => si.UnitPrice).HasPrecision(18, 2);
            entity.Property(si => si.SubTotal).HasPrecision(18, 2);

            entity.HasOne(si => si.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(si => si.Product)
                .WithMany()
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Purchase>(entity =>
        {
            entity.HasIndex(p => p.PurchaseNumber).IsUnique();
            entity.Property(p => p.TotalCost).HasPrecision(18, 2);

            entity.HasOne(p => p.Supplier)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PurchaseItem>(entity =>
        {
            entity.Property(pi => pi.UnitCost).HasPrecision(18, 2);
            entity.Property(pi => pi.SubTotal).HasPrecision(18, 2);

            entity.HasOne(pi => pi.Purchase)
                .WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Demo Data
        var category1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var category2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        builder.Entity<Category>().HasData(
            new Category { Id = category1Id, Name = "Electronics", Description = "Electronic devices and accessories", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Category { Id = category2Id, Name = "Office Supplies", Description = "Stationery and office equipment", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        var supplier1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        
        builder.Entity<Supplier>().HasData(
            new Supplier { Id = supplier1Id, CompanyName = "TechWholesale Inc.", ContactPerson = "John Doe", Email = "contact@techwholesale.com", Phone = "123-456-7890", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        builder.Entity<Product>().HasData(
            new Product 
            { 
                Id = Guid.Parse("44444444-4444-4444-4444-444444444441"), 
                Name = "Wireless Mouse", 
                SKU = "WM-001", 
                Description = "Ergonomic wireless mouse", 
                UnitPrice = 25.99m, 
                QuantityInStock = 50, 
                CategoryId = category1Id, 
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product 
            { 
                Id = Guid.Parse("44444444-4444-4444-4444-444444444442"), 
                Name = "Mechanical Keyboard", 
                SKU = "MK-002", 
                Description = "RGB Mechanical Keyboard", 
                UnitPrice = 89.99m, 
                QuantityInStock = 20, 
                CategoryId = category1Id, 
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product 
            { 
                Id = Guid.Parse("44444444-4444-4444-4444-444444444443"), 
                Name = "A4 Printer Paper", 
                SKU = "PP-A4", 
                Description = "500 sheets of A4 paper", 
                UnitPrice = 5.49m, 
                QuantityInStock = 200, 
                CategoryId = category2Id, 
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
