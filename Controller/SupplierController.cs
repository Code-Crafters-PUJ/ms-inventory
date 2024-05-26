using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly AppDbContext _context;

    public SupplierController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("company/{id}")]
    public async Task<IActionResult> GetSuppliersByCompany(int id)
    {
        var suppliers = await _context.Supplier
            .Where(s => s.CompanyId == id)
            .Include(s => s.SupplierType)
            .Include(s => s.ServiceType)
            .ToListAsync();

        var supplierDtos = suppliers.Select(s => new
        {
            id = s.SupplierId,
            name = s.Name,
            supplierType = s.SupplierType?.Name,
            address = s.Address,
            phone = s.Phone,
            email = s.Email,
            urlPage = s.UrlPage,
            serviceType = s.ServiceType?.Name
        }).ToList();

        return Ok(new { message = "Supplier retrieved successfully", data = supplierDtos });
    }
    
      [HttpGet("company/{id}/{supplierID}")]
    public async Task<IActionResult> GetSupplierByCompanyAndId(int id, int supplierID)
    {
        if (id <= 0 || supplierID <= 0)
        {
            return BadRequest(new { message = "Invalid ID supplied or Bad request" });
        }

        var supplier = await _context.Supplier
            .Include(s => s.SupplierType)
            .Include(s => s.ServiceType)
            .FirstOrDefaultAsync(s => s.CompanyId == id && s.SupplierId == supplierID);

        if (supplier == null)
        {
            return NotFound(new { message = "Supplier not found" });
        }

        var supplierDto = new
        {
            id = supplier.SupplierId,
            name = supplier.Name,
            supplierType = supplier.SupplierType?.Name,
            address = supplier.Address,
            phone = supplier.Phone,
            email = supplier.Email,
            urlPage = supplier.UrlPage,
            serviceType = supplier.ServiceType?.Name
        };

        return Ok(new { message = "Supplier retrieved successfully", data = new[] { supplierDto } });
    }

   [HttpPut("company/{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] SupplierUpdateDto supplierDto)
    {
        if (id <= 0 || supplierDto.id <= 0)
        {
            return BadRequest(new { message = "Invalid ID supplied or Bad request" });
        }

        var existingSupplier = await _context.Supplier
            .FirstOrDefaultAsync(s => s.CompanyId == id && s.SupplierId == supplierDto.id);

        if (existingSupplier == null)
        {
            return NotFound(new { message = "Supplier not found" });
        }

        var duplicateSupplier = await _context.Supplier
            .FirstOrDefaultAsync(s => s.CompanyId == id && s.Name == supplierDto.name && s.SupplierId != supplierDto.id);

        if (duplicateSupplier != null)
        {
            return Conflict(new { message = "Conflict: Duplicate entry" });
        }

        existingSupplier.Name = supplierDto.name;
        existingSupplier.Address = supplierDto.address;
        existingSupplier.Phone = supplierDto.phone;
        existingSupplier.Email = supplierDto.email;
        existingSupplier.UrlPage = supplierDto.urlPage;
        
        // Find and assign SupplierType
        var supplierType = await _context.SupplierType.FirstOrDefaultAsync(st => st.Name == supplierDto.supplierType);
        if (supplierType == null)
        {
            return BadRequest(new { message = "Invalid supplier type" });
        }
        existingSupplier.SupplierTypeId = supplierType.SupplierTypeId;

        // Find and assign ServiceType
        var serviceType = await _context.ServiceType.FirstOrDefaultAsync(st => st.Name == supplierDto.serviceType);
        if (serviceType == null)
        {
            return BadRequest(new { message = "Invalid service type" });
        }
        existingSupplier.ServiceTypeId = serviceType.ServiceTypeId;

        _context.Supplier.Update(existingSupplier);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Supplier updated successfully" });
    }

    [HttpGet("supplierId/company/{id}")]
    public async Task<IActionResult> GetValidSupplierId(int id)
    {
        var supplierId = await _context.Supplier.Where(s => s.CompanyId == id).Select(s => s.SupplierId).MaxAsync();
        return Ok(new { message = "Code generated successfully", supplierId = supplierId + 1 });
    }

[HttpPost("company/{id}")]
public async Task<IActionResult> AddSupplier(int id, [FromBody] SupplierCreateDto supplierDto)
{
    if (id <= 0)
    {
        return BadRequest(new { message = "Invalid Company ID supplied or Bad request" });
    }

    var duplicateSupplier = await _context.Supplier
        .FirstOrDefaultAsync(s => s.CompanyId == id && s.Name == supplierDto.name);

    if (duplicateSupplier != null)
    {
        return Conflict(new { message = "Conflict: Duplicate entry" });
    }

    var supplierType = await _context.SupplierType.FirstOrDefaultAsync(st => st.Name == supplierDto.supplierType);
    if (supplierType == null)
    {
        return BadRequest(new { message = "Invalid supplier type" });
    }

    var serviceType = await _context.ServiceType.FirstOrDefaultAsync(st => st.Name == supplierDto.serviceType);
    if (serviceType == null)
    {
        return BadRequest(new { message = "Invalid service type" });
    }

    var newSupplier = new Supplier
    {
        SupplierId = supplierDto.id, // Asignar el ID proporcionado
        Name = supplierDto.name,
        Address = supplierDto.address,
        Phone = supplierDto.phone,
        Email = supplierDto.email,
        UrlPage = supplierDto.urlPage,
        CompanyId = id,
        SupplierTypeId = supplierType.SupplierTypeId,
        ServiceTypeId = serviceType.ServiceTypeId
    };

    _context.Supplier.Add(newSupplier);
    await _context.SaveChangesAsync();

    return Created("", new { message = "Supplier added successfully" });
}



     [HttpDelete("supplier/company/{id}/{supplierId}")]
    public async Task<IActionResult> DeleteSupplier(int id, int supplierId)
    {
        if (id < 0 || supplierId < 0)
        {
            return BadRequest(new { message = "Invalid ID supplied" });
        }

        var supplier = await _context.Supplier
            .FirstOrDefaultAsync(s => s.CompanyId == id && s.SupplierId == supplierId);

        if (supplier == null)
        {
            return NotFound(new { message = "Supplier not found" });
        }

        _context.Supplier.Remove(supplier);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
