//using Data_Access.Context;
//using Data_Access.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//namespace AVTOSHOPWebApi.Controllers
//{
//    [Authorize]
//    [ApiController]
//    [Route("api/[controller]")]
//    public class FavoritesController : ControllerBase
//    {
//        private readonly CarContext _context;

//        public FavoritesController(CarContext context)
//        {
//            _context = context;
//        }

//        // GET api/favorites
//        [HttpGet]
//        public async Task<IActionResult> GetFavorites()
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var favoriteCars = await _context.Favorites
//                .Where(f => f.UserId == userId)
//                .Select(f => f.Car)
//                .ToListAsync();

//            return Ok(favoriteCars);
//        }

//        // POST api/favorites/{carId}
//        [HttpPost("{carId}")]
//        public async Task<IActionResult> AddFavorite(int carId)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            var exists = await _context.Favorites.AnyAsync(f => f.UserId == userId && f.CarId == carId);
//            if (exists)
//                return BadRequest("Already in favorites");

//            var favorite = new Favorite
//            {
//                UserId = userId,
//                CarId = carId
//            };

//            _context.Favorites.Add(favorite);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }

//        // DELETE api/favorites/{carId}
//        [HttpDelete("{carId}")]
//        public async Task<IActionResult> RemoveFavorite(int carId)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            var favorite = await _context.Favorites
//                .FirstOrDefaultAsync(f => f.UserId == userId && f.CarId == carId);

//            if (favorite == null)
//                return NotFound();

//            _context.Favorites.Remove(favorite);
//            await _context.SaveChangesAsync();

//            return Ok();
//        }
//    }

//}
