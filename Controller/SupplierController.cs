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

    // GET: api/Suppliers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
    {
        return await _context.Supplier.ToListAsync();
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

    // PUT: api/Suppliers/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSupplier(int id, Supplier supplier)
    {
        if (id != supplier.SupplierId)
        {
            return BadRequest();
        }

        _context.Entry(supplier).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SupplierExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Suppliers 
    [HttpPost]
    public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
    {
        do
        {
            // Generate a random, unique SupplierId
            supplier.SupplierId = GenerateUniqueSupplierId();
        } while (_context.Supplier.Any(e => e.SupplierId == supplier.SupplierId));

        _context.Supplier.Add(supplier);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplier);
    }

    // DELETE: api/Suppliers/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<Supplier>> DeleteSupplier(int id)
    {
        var supplier = await _context.Supplier.FindAsync(id);
        if (supplier == null)
        {
            return NotFound();
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

        newId = new Random().Next(1, int.MaxValue); // Ensure positive ID
    } while (_context.Supplier.Any(e => e.SupplierId == newId));

    return newId;
}

}
