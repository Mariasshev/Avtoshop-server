using Data_Access.Context;
using Data_Access.Models;
using Data_Transfer_Object.DTO.Saler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalersController : ControllerBase
    {
        private readonly CarContext _context;

        public SalersController(CarContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddSaler([FromBody] SalerDto dto)
        {
            var saler = new Saler
            {
                Name = dto.Name,
                Photo = dto.Photo,
                Number = dto.Number,
                Email = dto.Email,
                Adress = dto.Adress
            };

            _context.Salers.Add(saler);
            await _context.SaveChangesAsync();

            return Ok(saler);
        }
    }
}
