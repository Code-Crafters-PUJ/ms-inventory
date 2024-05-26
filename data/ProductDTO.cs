public class ProductDTO
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ProductDTO()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Description = string.Empty; // Inicializar Description aquí
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double CostPrice { get; set; }
    public double SalePrice { get; set; }
    public string Category { get; set; }
    public List<BranchProductDTO> Branches { get; set; }
}
