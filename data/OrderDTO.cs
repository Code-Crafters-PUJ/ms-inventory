public class OrderDTO
{
    public required string ProductName { get; set; }
    public required string SupplierName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public double CostPrice { get; set; }
    public required List<BranchOrderDTO> Branches { get; set; }
}
