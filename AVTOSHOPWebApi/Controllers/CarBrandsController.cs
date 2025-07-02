using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data_Access.Context;
using System.Linq;
using System.Threading.Tasks;

namespace AVTOSHOPWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarBrandsController : ControllerBase
    {
        private readonly CarContext _context;

        public CarBrandsController(CarContext context)
        {
            _context = context;
        }

        // GET: api/CarBrands
        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _context.Brands
                .Select(b => new { b.Id, b.Name })
                .ToListAsync();

            return Ok(brands);
        }

        // GET: api/CarBrands/{brandId}/models
        [HttpGet("{brandId}/models")]
        public async Task<IActionResult> GetModelsByBrand(int brandId)
        {
            var models = await _context.Models
                .Where(m => m.BrandId == brandId)
                .Select(m => new { m.Id, m.Name })
                .ToListAsync();

            return Ok(models);
        }
    }
}
