using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

 
    public DbSet<Branch> Branch { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Supplier> Supplier { get; set; }
    public DbSet<SupplierType> SupplierType { get; set; }
    public DbSet<ServiceType> ServiceType { get; set; }
    public DbSet<BranchHasProduct> BranchHasProduct { get; set; }
    public DbSet<ProductHasSupplier> ProductHasSupplier { get; set; }
    public DbSet<Company> Company { get; set; }
       protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BranchHasProduct>()
            .HasKey(bc => new { bc.BranchId, bc.ProductId });

        modelBuilder.Entity<ProductHasSupplier>()
            .HasKey(ps => new { ps.ProductId, ps.SupplierId, ps.PurchaseDate, ps.CostPrice, ps.Quantity, ps.OrderId, ps.BranchId });

        modelBuilder.Entity<Supplier>()
            .Property(s => s.SupplierId)
            .ValueGeneratedNever(); // Esto permite que el ID sea asignado manualmente
    }

}