
namespace MicroservicesEcosystem.Repositories.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        Task<List<T>> GetAll();
        Task<T> Get(Guid id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(Guid id);
        Task RollbackTransactionAsync();
        Task<IDisposable> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task SaveChangesAsync();

        Task UpdateDetached(T entity);
    }
}
