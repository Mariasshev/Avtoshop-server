namespace Data_Access.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task Add(T entity);
        void Update(T entity);
        Task Delete(int id);
        Task<T?> GetById(int? id);
    }

}
