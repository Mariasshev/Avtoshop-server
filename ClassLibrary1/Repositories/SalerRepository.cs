using Data_Access.Context;
using Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Data_Access.Repositories
{
    public interface ISalerRepository
    {
        Task<Saler?> GetSalerByUserIdAsync(int userId);
        Task UpdateAsync(Saler saler);
    }

    public class SalerRepository : ISalerRepository
    {
        private readonly CarContext _context;

        public SalerRepository(CarContext context)
        {
            _context = context;
        }

        public async Task<Saler?> GetSalerByUserIdAsync(int userId)
        {
            return await _context.Salers.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task UpdateAsync(Saler saler)
        {
            _context.Salers.Update(saler);
            await _context.SaveChangesAsync();
        }
    }
}
