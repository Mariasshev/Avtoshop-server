using Data_Access.Context;
using Data_Access.Models;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Repositories
{

    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();

        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateAsync(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly CarContext _context;

        public UserRepository(CarContext context) => _context = context;

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }




}
