using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Data_Access.Models;
using Data_Access;
using Data_Access.Repositories;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


using BLL.Interfaces;
using Data_Transfer_Object.DTO.UserDTO;
using System.Linq;

namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = "ОченьДлинный_Секретный_Ключ_Минимум32символа";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                // другие claims
            };

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            var result = await _userService.RegisterAsync(dto);
            if (!result) return BadRequest("Пользователь с таким email уже существует.");
            return Ok(new { message = "Успешная регистрация", name = dto.Name });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var user = await _userService.ValidateUserAsync(dto);

            if (user == null)
                return Unauthorized("Неверный email или пароль");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                name = user.Name,
                message = "Успешная авторизация"
            });
        }

    }

}
