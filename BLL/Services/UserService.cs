using Microsoft.AspNetCore.Identity;
using Data_Access.Models;
using Data_Access;
using BLL.Interfaces;
using Data_Transfer_Object.DTO.UserDTO;
using Data_Access.Repositories;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(UserRegisterDTO dto)
        {
            var existing = await _userRepository.FindByEmailAsync(dto.Email);
            if (existing != null) return false;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                isAdmin = false
            };

            user.Password = _hasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        //public async Task<string?> LoginAsync(UserLoginDTO dto)
        //{
        //    var user = await _userRepository.FindByEmailAsync(dto.Email);
        //    if (user == null) return null;

        //    var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
        //    return result == PasswordVerificationResult.Success ? "TOKEN_FAKE" : null;
        //}
        public async Task<LoginResult> LoginAsync(UserLoginDTO dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            if (user == null)
                return new LoginResult { Success = false, ErrorMessage = "Пользователь с таким email не найден" };

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result != PasswordVerificationResult.Success)
                return new LoginResult { Success = false, ErrorMessage = "Неверный пароль" };

            // Тут должен быть настоящий JWT токен, пока заглушка
            var token = "TOKEN_FAKE";

            return new LoginResult { Success = true, Token = token, Name = user.Name };
        }



    }


}
