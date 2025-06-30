using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Data_Access.Repositories;
using System.Security.Claims;
using Data_Transfer_Object.DTO.UserDTO;

namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            // Логируем все клеймы
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
            }

            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"Найденный email (по NameIdentifier): {email}");

            if (email == null) return Unauthorized();

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound();

            return Ok(new
            {
                name = user.Name,
                surname = user.Surname,
                email = user.Email,
                phoneNumber = user.PhoneNumber,
                city = user.City,
                country = user.Country
            });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto dto)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($">>> UpdateProfile called for: {email}");

                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine(">>> Email не найден в токене");
                    return Unauthorized("Пользователь не найден.");
                }

                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                {
                    Console.WriteLine(">>> Пользователь не найден в базе");
                    return NotFound("Пользователь не найден.");
                }

                // Обновляем только те поля, которые разрешено менять
                user.Name = dto.Name ?? user.Name;
                user.Surname = dto.Surname ?? user.Surname;
                user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
                user.City = dto.City ?? user.City;
                user.Country = dto.Country ?? user.Country;

                // Email обычно не меняют, но если тебе нужно:
                // user.Email = dto.Email ?? user.Email;

                await _userRepository.UpdateAsync(user);

                Console.WriteLine(">>> Данные успешно обновлены");
                return Ok(new { message = "Данные успешно обновлены!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Ошибка при обновлении профиля: {ex.Message}");
                return StatusCode(500, "Ошибка сервера при обновлении профиля.");
            }
        }


    }
}
