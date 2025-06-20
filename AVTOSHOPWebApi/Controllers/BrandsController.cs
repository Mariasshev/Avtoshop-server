using Data_Access.Context;
using Data_Access.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;

namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly CarContext _context;

        public BrandsController(CarContext context)
        {
            _context = context;
        }

        // POST api/brands
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] CarBrands brand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrandById), new { id = brand.Id }, brand);
        }

        // GET api/brands/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                return NotFound();

            return Ok(brand);
        }
    }

}
