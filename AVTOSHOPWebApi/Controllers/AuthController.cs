using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.Interfaces;
using Data_Transfer_Object.DTO.UserDTO;
using Data_Access.Models;


namespace AVTOSHOPWebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
        {
            var success = await _userService.RegisterAsync(dto);
            if (!success)
                return BadRequest("Email already registered");

            var user = await _userService.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return StatusCode(500, "User registration error");

            var token = GenerateJwtToken(user.Email, user.Id);
            Console.WriteLine($"New user ID: {user.Id}");


            return Ok(new { token, name = user.Name, id = user.Id });

        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var user = await _userService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user.Email, user.Id);
            Console.WriteLine($"New user ID: {user.Id}");

            //return Ok(new { token, name = user.Name });
            return Ok(new { token, name = user.Name, id = user.Id });
        }

        private string GenerateJwtToken(string email, int userId)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),         // email как Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", userId.ToString())                  // id пользователя под кастомным именем
            };


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}

