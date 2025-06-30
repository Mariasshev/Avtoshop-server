//using Microsoft.AspNetCore.Mvc;
//using Data_Access.Models;
//using BLL.Interfaces;
//using Data_Transfer_Object.DTO.UserDTO;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace AVTOSHOPWebApi.Controllers
//{
//    [ApiController]
//    [Route("api/auth")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly IConfiguration _configuration;

//        public AuthController(IUserService userService, IConfiguration configuration)
//        {
//            _userService = userService;
//            _configuration = configuration;
//        }

//        private string GenerateJwtToken(User user)
//        {
//            var jwtSettings = _configuration.GetSection("JwtSettings");
//            var secretKey = jwtSettings["Key"];
//            var issuer = jwtSettings["Issuer"];
//            var audience = jwtSettings["Audience"];

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var claims = new[]
//            {
//                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
//            };

//            var token = new JwtSecurityToken(
//                issuer: issuer,
//                audience: audience,
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(1),
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] UserRegisterDTO dto)
//        {
//            var result = await _userService.RegisterAsync(dto);
//            if (!result) return BadRequest("Пользователь с таким email уже существует.");
//            return Ok(new { message = "Успешная регистрация", name = dto.Name });
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
//        {
//            var user = await _userService.ValidateUserAsync(dto);

//            if (user == null)
//                return Unauthorized("Неверный email или пароль");

//            var token = GenerateJwtToken(user);

//            return Ok(new
//            {
//                token,
//                name = user.Name,
//                message = "Успешная авторизация"
//            });
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BLL.Interfaces;
using Data_Transfer_Object.DTO.UserDTO;

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

            var token = GenerateJwtToken(dto.Email);
            return Ok(new { token, name = dto.Name });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            var user = await _userService.ValidateUserAsync(dto);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user.Email);
            return Ok(new { token, name = user.Name });
        }

        private string GenerateJwtToken(string email)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

