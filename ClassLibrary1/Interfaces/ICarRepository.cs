using Data_Access.Models;
namespace Data_Access.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        void Update(Car car);
        void Delete(Car car);
        Task SaveChangesAsync();
    }
}
