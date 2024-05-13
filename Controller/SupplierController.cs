using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController : ControllerBase
{
    private readonly AppDbContext _context;

    public SuppliersController(AppDbContext context)
    {
        _context = context;
    }

   // GET: api/Suppliers/company/1
    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersByCompanyId(int companyId)
    {
        var supplier = await _context.Supplier
            .Where(b => b.CompanyId == companyId)
            .ToListAsync();

        if (supplier == null || supplier.Count == 0)
        {
            return NotFound();
        }

        return supplier;
    }
    // GET: api/Suppliers/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetSupplier(int id)
    {
        var supplier = await _context.Supplier.FindAsync(id);

        if (supplier == null)
        {
            return NotFound();
        }

        return supplier;
    }

// PUT: api/Suppliers/1
[HttpPut("{companyId}/{supplierId}")]
public async Task<IActionResult> PutSupplier(int companyId, int supplierId, [FromBody] Supplier supplier)
{
    if (companyId != supplier.CompanyId || supplierId != supplier.SupplierId)
    {
        return BadRequest("Company ID or Supplier ID in the request body does not match the URL");
    }

    var company = await _context.Company.FindAsync(companyId);
    if (company == null)
    {
        return NotFound("Company not found");
    }

    var existingSupplier = await _context.Supplier.FindAsync(supplierId);
    if (existingSupplier == null)
    {
        return NotFound("Supplier not found");
    }

        // Check if SupplierType already exists, if not, add it
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var existingSupplierType = _context.SupplierType.FirstOrDefault(st => st.Name == supplier.SupplierType.Name);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (existingSupplierType == null)
    {
        supplier.SupplierType = null; // Remove SupplierType to prevent creation
    }
    else
    {
        supplier.SupplierTypeId = existingSupplierType.SupplierTypeId;
        supplier.SupplierType = existingSupplierType;
    }

        // Check if ServiceType already exists, if not, add it
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var existingServiceType = _context.ServiceType.FirstOrDefault(st => st.Name == supplier.ServiceType.Name);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (existingServiceType == null)
    {
        supplier.ServiceType = null; // Remove ServiceType to prevent creation
    }
    else
    {
        supplier.ServiceTypeId = existingServiceType.ServiceTypeId;
        supplier.ServiceType = existingServiceType;
    }


    existingSupplier.Name = supplier.Name;
    existingSupplier.Address = supplier.Address;
    existingSupplier.Phone = supplier.Phone;
    existingSupplier.Email = supplier.Email;
    existingSupplier.UrlPage = supplier.UrlPage;


    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!SupplierExists(supplierId))
        {
            return NotFound("Supplier not found");
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}



   // POST: api/Suppliers/1
    [HttpPost("{companyId}")]
    public async Task<ActionResult<Supplier>> PostSupplier(int companyId, Supplier supplier)
    {
        var company = await _context.Company.FindAsync(companyId);
        if (company == null)
        {
            return NotFound("Company not found");
        }

       
        supplier.CompanyId = companyId;


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var existingSupplierType = _context.SupplierType.FirstOrDefault(st => st.Name == supplier.SupplierType.Name);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (existingSupplierType == null)
        {
            supplier.SupplierType = null; // Remove SupplierType to prevent creation
        }
        else
        {
            supplier.SupplierTypeId = existingSupplierType.SupplierTypeId;
            supplier.SupplierType = existingSupplierType;
        }

        // Check if ServiceType already exists, if not, add it
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var existingServiceType = _context.ServiceType.FirstOrDefault(st => st.Name == supplier.ServiceType.Name);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (existingServiceType == null)
        {
            supplier.ServiceType = null; // Remove ServiceType to prevent creation
        }
        else
        {
            supplier.ServiceTypeId = existingServiceType.ServiceTypeId;
            supplier.ServiceType = existingServiceType;
        }

        do
        {
            // Generate a random, unique SupplierId
            supplier.SupplierId = GenerateUniqueSupplierId();
        } while (_context.Supplier.Any(e => e.SupplierId == supplier.SupplierId));

        _context.Supplier.Add(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplier);
    }

// DELETE: api/Suppliers/1/2
[HttpDelete("{companyId}/{supplierId}")]
public async Task<ActionResult<Supplier>> DeleteSupplier(int companyId, int supplierId)
{
    var company = await _context.Company.FindAsync(companyId);
    if (company == null)
    {
        return NotFound("Company not found");
    }

    var supplier = await _context.Supplier.FindAsync(supplierId);
    if (supplier == null)
    {
        return NotFound("Supplier not found");
    }

    _context.Supplier.Remove(supplier);
    await _context.SaveChangesAsync();

    return supplier;
}


    private bool SupplierExists(int id)
    {
        return _context.Supplier.Any(e => e.SupplierId == id);
    }

private int GenerateUniqueSupplierId()
{
    int newId;
    do
    {

        newId = new Random().Next(1, int.MaxValue); 
    } while (_context.Supplier.Any(e => e.SupplierId == newId));

    return newId;
}


}
