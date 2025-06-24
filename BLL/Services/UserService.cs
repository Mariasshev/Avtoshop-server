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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
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
        public async Task<User?> ValidateUserAsync(UserLoginDTO dto)
        {
            var user = await _userRepository.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            return result == PasswordVerificationResult.Success ? user : null;
        }




    }


}
