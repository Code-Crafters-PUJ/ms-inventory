using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[Route("api/[controller]")]
[ApiController]
public class CompanyController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompanyController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/company/1/branches
    //Esta solicitud permite obtener todas las sucursales de una compañía por su ID. Devuelve un JSON que contiene la lista de sucursales.
    [HttpGet("{companyId}/branches")]
    public async Task<ActionResult<IEnumerable<Branch>>> GetBranchesByCompanyId(int companyId)
    {
        var branches = await _context.Branch
            .Where(b => b.CompanyId == companyId)
            .ToListAsync();

        if (branches == null || branches.Count == 0)
        {
            return NotFound();
        }

        return branches;
    }

    // GET: api/company/1/branches/names
    //Esta solicitud permite obtener los nombres de todas las sucursales de una compañía por su ID. Devuelve un JSON que contiene una lista de nombres de sucursales.
    [HttpGet("{companyId}/branches/names")]
    public async Task<ActionResult<IEnumerable<string>>> GetBranchNamesByCompanyId(int companyId)
    {
        var branchNames = await _context.Branch
            .Where(b => b.CompanyId == companyId)
            .Select(b => b.Name)
            .ToListAsync();

        if (branchNames == null || branchNames.Count == 0)
        {
            return NotFound();
        }

#pragma warning disable CS8619 
        return branchNames;
#pragma warning restore CS8619 
    }

// POST: api/company/1/products 
//Esta solicitud permite crear un nuevo producto para una compañía específica. Se debe enviar los detalles del nuevo producto en el cuerpo de la solicitud.
[HttpPost("company/{companyId}/products")]
    public async Task<IActionResult> CreateProductForCompany(int companyId, [FromBody] Product productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var branches = await _context.Branch
            .Where(b => b.CompanyId == companyId)
            .ToListAsync();

        if (branches == null || !branches.Any())
        {
            return NotFound($"No se encontraron sucursales para la compañía con el ID {companyId}");
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    CostPrice = productDto.CostPrice,
                    SalePrice = productDto.SalePrice,
                    CategoryId = productDto.CategoryId
                };

                _context.Product.Add(product);
                await _context.SaveChangesAsync();


                foreach (var branch in branches)
                {
                    var branchHasProduct = new BranchHasProduct
                    {
                        BranchId = branch.BranchId,
                        ProductId = product.ProductId,
                        Quantity = 0, 
                        Discount = 0
                    };

                    _context.BranchHasProduct.Add(branchHasProduct);
                }

                await _context.SaveChangesAsync();
                transaction.Commit();

                return Ok(product);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return StatusCode(500, $"Error al guardar el producto: {ex.Message}");
            }
        }
    }


// GET: api/company/1/products/6/branches
// Esta solicitud permite obtener todas las sucursales relacionadas con un producto específico de una compañía. Se devuelve un JSON que contiene la lista de sucursales con información relevante sobre la relación del producto con cada sucursal.
[HttpGet("{companyId}/products/{productId}/branches")]
public async Task<ActionResult<IEnumerable<BranchHasProduct>>> GetBranchesByProductId(int companyId, int productId)
{
#pragma warning disable CS8602 
        var branchesWithProduct = await _context.BranchHasProduct
        .Include(bhp => bhp.Branch)
        .Include(bhp => bhp.Product)
        .Where(bhp => bhp.ProductId == productId && bhp.Branch.CompanyId == companyId)
        .ToListAsync();
#pragma warning restore CS8602 

        if (branchesWithProduct == null || branchesWithProduct.Count == 0)
    {
        return NotFound();
    }

    return branchesWithProduct;
}


// DELETE: api/company/1/products/6
// Esta solicitud permite eliminar un producto específico de una compañía. Se debe enviar el ID del producto en la URL de la solicitud.
[HttpDelete("{companyId}/products/{productId}")]
public IActionResult DeleteProduct(int companyId, int productId)
{

#pragma warning disable CS8602 
        var branchProductRelations = _context.BranchHasProduct
        .Where(bhp => bhp.ProductId == productId && bhp.Branch.CompanyId == companyId)
        .ToList();
#pragma warning restore CS8602 

        if (branchProductRelations == null || branchProductRelations.Count == 0)
    {
        return NotFound($"No se encontraron relaciones del producto con el ID {productId} para la compañía con el ID {companyId}");
    }


    _context.BranchHasProduct.RemoveRange(branchProductRelations);


    _context.SaveChanges();

    return NoContent();
}


// PUT: api/company/1/products/8
//Esta solicitud permite actualizar un producto existente de una compañía. Se debe enviar los detalles actualizados del producto en el cuerpo de la solicitud.
[HttpPut("company/{companyId}/products/{productId}")]
public async Task<IActionResult> UpdateProductForCompany(int companyId, int productId, [FromBody] Product productDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var product = await _context.Product.FindAsync(productId);
    if (product == null)
    {
        return NotFound($"No se encontró el producto con el ID {productId}");
    }

    var branches = await _context.Branch
        .Where(b => b.CompanyId == companyId)
        .ToListAsync();

    if (branches == null || !branches.Any())
    {
        return NotFound($"No se encontraron sucursales para la compañía con el ID {companyId}");
    }

    using (var transaction = _context.Database.BeginTransaction())
    {
        try
        {
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.CostPrice = productDto.CostPrice;
            product.SalePrice = productDto.SalePrice;
            product.CategoryId = productDto.CategoryId;

            await _context.SaveChangesAsync();

            // Eliminar relaciones de sucursales existentes para este producto
            var existingRelations = await _context.BranchHasProduct
                .Where(bhp => bhp.ProductId == productId)
                .ToListAsync();

            foreach (var relation in existingRelations)
            {
                _context.BranchHasProduct.Remove(relation);
            }

            await _context.SaveChangesAsync();

            // Crear nuevas relaciones de sucursales para este producto
            foreach (var branch in branches)
            {
                var branchHasProduct = new BranchHasProduct
                {
                    BranchId = branch.BranchId,
                    ProductId = productId,
                    Quantity = 0,
                    Discount = 0
                };

                _context.BranchHasProduct.Add(branchHasProduct);
            }

            await _context.SaveChangesAsync();
            transaction.Commit();

            return Ok(product);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return StatusCode(500, $"Error al actualizar el producto: {ex.Message}");
        }
    }
}



    private bool CompanyExists(int id)
    {
        return _context.Company.Any(e => e.CompanyId == id);
    }

// POST: api/company
//Metodo para insertar una compañia
[HttpPost]
public async Task<ActionResult<Company>> PostCompany(Company company)
{
    // Agregar la compañía a DbSet Company y guardar en la base de datos
    _context.Company.Add(company);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(PostCompany), new { id = company.CompanyId }, company);
}

   // POST: api/company/{companyId}/branches
   //Metodo para insertar una sucursal dentro de una compañia
    [HttpPost("{companyId}/branches")]
    public async Task<ActionResult<Branch>> PostBranch(int companyId, Branch branch)
    {
        var company = await _context.Company.FindAsync(companyId);
        if (company == null)
        {
            return NotFound($"Company with ID {companyId} not found.");
        }

        branch.CompanyId = companyId;

        
        _context.Branch.Add(branch);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostBranch), new { companyId, branchId = branch.BranchId }, branch);
    }

// GET: api/company
//Ver todas las compañias
[HttpGet]
public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
{
    var companies = await _context.Company.ToListAsync();

    if (companies == null || companies.Count == 0)
    {
        return NotFound();
    }

    return companies;
}

// DELETE: api/company/{id}
//Eliminar una compañia
[HttpDelete("{id}")]
public async Task<ActionResult<Company>> DeleteCompany(int id)
{
    var company = await _context.Company.FindAsync(id);
    if (company == null)
    {
        return NotFound($"Company with ID {id} not found.");
    }

    _context.Company.Remove(company);
    await _context.SaveChangesAsync();

    return company;
}

// POST: api/category
//Crear una categoria de producto
[HttpPost ("/category")]
public async Task<ActionResult<Category>> PostCategory(Category category)
{
    _context.Category.Add(category);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(PostCompany), new { id = category.CategoryId }, category);
}


    // GET api/orders/generateOrderId
    [HttpGet("generateOrderId")]
    public IActionResult GenerateOrderId()
    {
        var orderId = GenerateUniqueOrderId();
        return Ok(new { OrderId = orderId });
    }


    private string GenerateUniqueOrderId()
    {
        var orderId = Guid.NewGuid().ToString(); 
        return orderId;
    }


    
}
