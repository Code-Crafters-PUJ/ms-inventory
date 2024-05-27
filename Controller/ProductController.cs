using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

 [HttpGet("branches/company/{id}")]
public async Task<IActionResult> GetBranchesByCompanyId(int id)
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var branches = await _context.Branch
        .Where(b => b.CompanyId == id)
        .Select(b => new 
        {
            id = b.BranchId,
            name = b.Name,
            products = b.BranchHasProducts.Select(bp => new 
            {
                product = new 
                {
                    id = bp.Product.ProductId,
                    name = bp.Product.Name,
                    description = bp.Product.Description,
                    category = bp.Product.Category.Name,
                    sellPrice = bp.Product.SalePrice
                },
                quantity = bp.Quantity,
                discount = bp.Discount
            })
        }).ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (branches == null || !branches.Any())
        return Ok(new List<object>());

    return Ok(branches);
}


 [HttpGet("branches/names/company/{id}")]
public async Task<IActionResult> GetBranchNamesByCompanyId(int id)
{
    var branchNames = await _context.Branch
        .Where(b => b.CompanyId == id)
        .Select(b => new { name = b.Name })
        .ToListAsync();

    if (branchNames == null || !branchNames.Any())
        return Ok(new List<object>());

    return Ok(branchNames);
}


 [HttpPost("products/company/{id}")]
public async Task<IActionResult> CreateProductForCompany(int id, [FromBody] ProductDTO productDto)
{
    if (productDto == null)
        return BadRequest("Unable to add product");

    var category = await _context.Category.FirstOrDefaultAsync(c => c.Name == productDto.Category);
    if (category == null)
        return BadRequest("Invalid category");

    var product = new Product
    {
        Name = productDto.Name,
        Description = productDto.Description,
        CostPrice = productDto.CostPrice,
        SalePrice = productDto.SalePrice,
        CategoryId = category.CategoryId
    };

    _context.Product.Add(product);
    await _context.SaveChangesAsync();

                        var productRMQDTO = new ProductRMQDTO
                {
                    id = product.ProductId,
                    name = productDto.Name,
                    description = productDto.Description,
                    salePrice = productDto.SalePrice,
                    categoryId = category.CategoryId
                };

                ProductCreateUpdateProducer publisher = new ProductCreateUpdateProducer();
                await publisher.PublishMessage(productRMQDTO);

    foreach (var branchDto in productDto.Branches)
    {
        var branch = await _context.Branch.FirstOrDefaultAsync(b => b.Name == branchDto.BranchName && b.CompanyId == id);
        if (branch == null)
            return BadRequest($"Invalid branch name: {branchDto.BranchName}");

        var branchProduct = new BranchHasProduct
        {
            BranchId = branch.BranchId,
            ProductId = product.ProductId,
            Quantity = branchDto.Quantity,
            Discount = branchDto.Discount
        };

        _context.BranchHasProduct.Add(branchProduct);

        var branchProductRMQDTO = new BranchProductRMQDTO
        {
            branchId = branch.BranchId,
            productId = product.ProductId,
            quantity = branchDto.Quantity,
            discount = branchDto.Discount
        };
        BranchProductCreateUpdateProducer publisherBP = new BranchProductCreateUpdateProducer();
        await publisherBP.PublishMessage(branchProductRMQDTO);
    }

    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetProductById), new { id = id, productId = product.ProductId }, productDto);
}

  [HttpGet("products/company/{id}/{productId}/branches")]
public async Task<IActionResult> GetBranchesByProductId(int id, int productId)
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var branches = await _context.BranchHasProduct
        .Where(bp => bp.ProductId == productId && bp.Branch.CompanyId == id)
        .Select(bp => new 
        {
            branchName = bp.Branch.Name,
            quantity = bp.Quantity,
            discount = bp.Discount
        }).ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (branches == null || !branches.Any())
        return Ok(new List<object>());

    return Ok(branches);
}

 [HttpDelete("products/company/{id}/{productId}")]
public async Task<IActionResult> DeleteProduct(int id, int productId)
{
    var product = await _context.Product.FindAsync(productId);
    if (product == null)
        return NotFound("Product not found");

    _context.Product.Remove(product);
    await _context.SaveChangesAsync();

    return NoContent();
}

   [HttpPut("products/company/{id}/{productId}")]
public async Task<IActionResult> UpdateProduct(int id, int productId, [FromBody] ProductDTO productDto)
{
    if (productDto == null || productId != productDto.Id)
        return BadRequest("Invalid ID supplied or Bad request");

    var product = await _context.Product.FindAsync(productId);
    if (product == null)
        return NotFound("Product not found");

    var category = await _context.Category.FirstOrDefaultAsync(c => c.Name == productDto.Category);
    if (category == null)
        return BadRequest("Invalid category");

    product.Name = productDto.Name;
    product.Description = productDto.Description;
    product.CostPrice = productDto.CostPrice;
    product.SalePrice = productDto.SalePrice;
    product.CategoryId = category.CategoryId;

    _context.Entry(product).State = EntityState.Modified;

    var existingBranchProducts = await _context.BranchHasProduct.Where(bp => bp.ProductId == productId).ToListAsync();
    _context.BranchHasProduct.RemoveRange(existingBranchProducts);

                            var productRMQDTO = new ProductRMQDTO
                {
                    id = product.ProductId,
                    name = productDto.Name,
                    description = productDto.Description,
                    salePrice = productDto.SalePrice,
                    categoryId = category.CategoryId
                };

                ProductCreateUpdateProducer publisher = new ProductCreateUpdateProducer();
                await publisher.PublishMessage(productRMQDTO);

    foreach (var branchDto in productDto.Branches)
    {
        var branch = await _context.Branch.FirstOrDefaultAsync(b => b.Name == branchDto.BranchName && b.CompanyId == id);
        if (branch == null)
            return BadRequest($"Invalid branch name: {branchDto.BranchName}");

        var branchProduct = new BranchHasProduct
        {
            BranchId = branch.BranchId,
            ProductId = product.ProductId,
            Quantity = branchDto.Quantity,
            Discount = branchDto.Discount
        };

        _context.BranchHasProduct.Add(branchProduct);

        
        var branchProductRMQDTO = new BranchProductRMQDTO
        {
            branchId = branch.BranchId,
            productId = product.ProductId,
            quantity = branchDto.Quantity,
            discount = branchDto.Discount
        };
        BranchProductCreateUpdateProducer publisherBP = new BranchProductCreateUpdateProducer();
        await publisherBP.PublishMessage(branchProductRMQDTO);
    }

    await _context.SaveChangesAsync();

    return Ok("Product updated successfully");
}


   [HttpGet("product/company/{id}/{productId}")]
public async Task<IActionResult> GetProductById(int id, int productId)
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var product = await _context.Product
        .Where(p => p.ProductId == productId)
        .Select(p => new 
        {
            id = p.ProductId,
            name = p.Name,
            description = p.Description,
            category = p.Category.Name,
            sellPrice = p.SalePrice,
            branches = p.BranchHasProducts.Select(bp => new 
            {
                branchName = bp.Branch.Name,
                quantity = bp.Quantity,
                discount = bp.Discount
            })
        }).FirstOrDefaultAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (product == null)
        return NotFound("Product not found");

        return Ok(new
    {
        message = "product retrieved successfully",
        product
    });
}

[HttpGet("products/names/company/{id}")]
public async Task<IActionResult> GetProductNamesByCompanyId(int id)
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var productNames = await _context.Product
        .Where(p => p.BranchHasProducts.Any(bhp => bhp.Branch.CompanyId == id))
        .Select(p => new { productName = p.Name })
        .ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (productNames == null || !productNames.Any())
        return Ok(new List<object>());

    return Ok(new
    {
        message = "product retrieved successfully",
        productNames
    });
}



 [HttpGet("orders/supplier/company/{id}/{supplierId}")]
public async Task<IActionResult> GetOrdersBySupplier(int id, int supplierId)
{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var orders = await _context.ProductHasSupplier
        .Where(ps => ps.SupplierId == supplierId &&
                     ps.Product.BranchHasProducts.Any(bhp => bhp.Branch.CompanyId == id))
        .Select(ps => new 
        {
            productName = ps.Product.Name,
            supplierName = ps.Supplier.Name,
            costPrice = ps.CostPrice,
            purchaseDate = ps.PurchaseDate,
            branches = ps.Product.BranchHasProducts.Select(bp => new 
            {
                branchName = bp.Branch.Name,
                quantity = bp.Quantity
            })
        }).ToListAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        if (orders == null || !orders.Any())
        return Ok(new List<object>());

        return Ok(new
    {
        message = "order retrieved successfully",
        orders
    });
}


[HttpPost("order/company/{id}")]
public async Task<IActionResult> AddOrder(int id, [FromBody] OrderDTO orderDto)
{
    if (orderDto == null)
        return BadRequest("Unable to add order");

    var supplier = await _context.Supplier.FirstOrDefaultAsync(s => s.Name == orderDto.SupplierName && s.CompanyId == id);
    if (supplier == null)
        return BadRequest("Invalid supplier");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var product = await _context.Product.FirstOrDefaultAsync(p => p.Name == orderDto.ProductName &&
                                                                   p.BranchHasProducts.Any(bhp => bhp.Branch.CompanyId == id));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (product == null)
        return BadRequest("Invalid product");

    foreach (var branchDto in orderDto.Branches)
    {
        var branch = await _context.Branch.FirstOrDefaultAsync(b => b.Name == branchDto.BranchName && b.CompanyId == id);
        if (branch == null)
            return BadRequest($"Invalid branch name: {branchDto.BranchName}");

        var order = new ProductHasSupplier
        {
            ProductId = product.ProductId,
            SupplierId = supplier.SupplierId,
            PurchaseDate = orderDto.PurchaseDate,
            CostPrice = orderDto.CostPrice,
            Quantity = branchDto.Quantity,
            BranchId = branch.BranchId,
            OrderId = new Random().Next() // Generate a unique order ID or use another method
        };

        _context.ProductHasSupplier.Add(order);
    }

    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetOrdersBySupplier), new { id = id, supplierId = supplier.SupplierId }, orderDto);
}

    private bool ProductExists(int id)
    {
        return _context.Product.Any(e => e.ProductId == id);
    }
}
