using Data_Access.Context;
using Data_Access.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/brands/{brandId}/models")]
    public class ModelsController : ControllerBase
    {
        private readonly CarContext _context;

        public ModelsController(CarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarModel>>> GetModels(int brandId)
        {
            var models = await _context.Models.Where(m => m.BrandId == brandId).ToListAsync();
            return models;
        }
    }

}
