using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Data_Access.Models;
using Data_Access;
using Data_Access.Repositories;
using Microsoft.AspNetCore.Authorization;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


using BLL.Interfaces;
using Data_Transfer_Object.DTO.UserDTO;
using System.Linq;

namespace AVTOSHOPWebApi.Controllers {

    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            // получаем email из claims токена
            var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return Ok(new
            {
                Email = email,
                Message = "Доступ к профилю подтверждён JWT"
            });
        }
    }
}