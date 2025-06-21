using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Data_Access.Models;
using Data_Access;
using Data_Access.Repositories;

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
            var loginResult = await _userService.LoginAsync(dto);

            if (!loginResult.Success)
                return Unauthorized(loginResult.ErrorMessage);

            return Ok(new { token = loginResult.Token, name = loginResult.Name, message = "Успешная авторизация" });
        }



    }

}
