using Data_Transfer_Object.DTO.UserDTO;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserRegisterDTO dto);
        Task<LoginResult> LoginAsync(UserLoginDTO dto);
    }
}
