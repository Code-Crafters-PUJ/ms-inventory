using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Branch.cs
public class Branch
{
    [Key]
    public int BranchId { get; set; }

    [Required]
    [StringLength(45)]
    public string? Name { get; set; }

    [Required]
    [StringLength(45)]
    public string? Address { get; set; }

    [Required]
    public bool Enabled { get; set; } 

    public int CompanyId { get; set; }

    // Navigation property
    public ICollection<BranchHasProduct> BranchHasProducts { get; set; } = new List<BranchHasProduct>();
}

// Product.cs
public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [StringLength(45)]
    public string? Name { get; set; }

    [StringLength(45)]
    public string? Description { get; set; }

    [Required]
    public double CostPrice { get; set; }

    [Required]
    public double SalePrice { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    // Navigation property
    public ICollection<BranchHasProduct> BranchHasProducts { get; set; } = new List<BranchHasProduct>();
}

// BranchHasProduct.cs
public class BranchHasProduct
{
    [Key, Column(Order = 1)]
    public int BranchId { get; set; }

    [Key, Column(Order = 2)]
    public int ProductId { get; set; }

    [ForeignKey("BranchId")]
    public Branch? Branch { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Range(0, 999)]
    public int Discount { get; set; }
}

// ProductHasSupplier.cs
public class ProductHasSupplier
{
    [Key, Column(Order = 1)]
    public int ProductId { get; set; }

    [Key, Column(Order = 2)]
    public int SupplierId { get; set; }

    [Key, Column(Order = 3)]
    public DateTime PurchaseDate { get; set; }

    [Key, Column(Order = 4)]
    public double CostPrice { get; set; }

    [Key, Column(Order = 5)]
    public int Quantity { get; set; }

    [Key, Column(Order = 6)]
    public int OrderId { get; set; }

    [Key, Column(Order = 7)]
    public int BranchId { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }

    [ForeignKey("SupplierId")]
    public Supplier? Supplier { get; set; }
}


// Category.cs
public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(45)]
    public string? Name { get; set; }
}

// Supplier.cs
public class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    [Required]
    [StringLength(45)]
    public string? Name { get; set; }

    [StringLength(45)]
    public string ?Address { get; set; }

    [StringLength(45)]
    public string ?Phone { get; set; }

    [StringLength(45)]
    public string ?Email { get; set; }

    [StringLength(45)]
    public string ?UrlPage { get; set; }

    public int CompanyId { get; set; }

    public int SupplierTypeId { get; set; }

    [ForeignKey("SupplierTypeId")]
    public SupplierType? SupplierType { get; set; }

    public int ServiceTypeId { get; set; }

    [ForeignKey("ServiceTypeId")]
    public ServiceType ?ServiceType { get; set; }
}

// SupplierType.cs
public class SupplierType
{
    [Key]
    public int SupplierTypeId { get; set; }

    [Required]
    [StringLength(45)]
    public string? Name { get; set; }
}

// ServiceType.cs
public class ServiceType
{
    [Key]
    public int ServiceTypeId { get; set; }

    [Required]
    [StringLength(45)]
    public string ?Name { get; set; }
}

// Company.cs
public class Company
{
    [Key]
    public int CompanyId { get; set; }

    [Required]
    [StringLength(45)]
    public string ?NIT { get; set; }

    [StringLength(45)]
    public string? businessArea { get; set; }

    [StringLength(45)]
    public string ?employeeNumber { get; set; }


}