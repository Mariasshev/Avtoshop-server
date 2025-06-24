using Data_Access.Models;
using Data_Transfer_Object.DTO.UserDTO;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserRegisterDTO dto);
        Task<User?> ValidateUserAsync(UserLoginDTO dto);

        Task<User?> GetUserByEmailAsync(string email);

    }
}
