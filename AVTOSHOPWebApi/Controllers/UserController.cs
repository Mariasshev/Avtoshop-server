using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Data_Access.Repositories;
using System.Security.Claims;
using Data_Transfer_Object.DTO.UserDTO;
using Data_Access.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ISalerRepository _salerRepository;

        public UserController(IUserRepository userRepository, ISalerRepository salerRepository)
        {
            _userRepository = userRepository;
            _salerRepository = salerRepository;
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
            //var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
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
                country = user.Country,
                photoUrl = user.PhotoUrl
            });
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto dto)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                //var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
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
                //user.PhotoUrl = dto.PhotoUrl ?? user.PhotoUrl;

                // Email обычно не меняют, но если тебе нужно:
                // user.Email = dto.Email ?? user.Email;

                await _userRepository.UpdateAsync(user);

                // Теперь обновляем Saler
                var saler = await _salerRepository.GetSalerByUserIdAsync(user.Id);
                if (saler != null)
                {
                    saler.Name = dto.Name ?? saler.Name;
                    saler.Number = dto.PhoneNumber ?? saler.Number;
                    saler.Adress = $"{dto.City}, {dto.Country}";
                    saler.Photo = user.PhotoUrl ?? saler.Photo;

                    await _salerRepository.UpdateAsync(saler);
                }


                Console.WriteLine(">>> Данные успешно обновлены и в таблице Salers");
                return Ok(new { message = "Данные успешно обновлены!" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Ошибка при обновлении профиля: {ex.Message}");
                return StatusCode(500, "Ошибка сервера при обновлении профиля.");
            }
        }



        //
        [HttpPut("profile/photo")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            // Получаем email из токена
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (email == null)
                return Unauthorized();

            // Ищем пользователя в базе
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("Пользователь не найден");

            try
            {
                // Генерируем имя файла, например, с GUID и расширением
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "profile_photos");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Сохраняем файл на диск
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Сохраняем путь к фото в базе (относительный путь)
                user.PhotoUrl = $"/uploads/profile_photos/{fileName}";
                await _userRepository.UpdateAsync(user);

                // Отдаем клиенту путь к фото (можно полный URL, если нужно)
                return Ok(new { photoUrl = user.PhotoUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки фото: {ex.Message}");
                return StatusCode(500, "Ошибка сервера при загрузке фото");
            }
        }



        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
           
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound("Пользователь не найден");

            // Проверка текущего пароля
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Текущий пароль неверный");

            // Устанавливаем новый пароль
            user.Password = hasher.HashPassword(user, dto.NewPassword);
            await _userRepository.UpdateAsync(user);

            return Ok("Пароль успешно изменён");
        }

    }
}
