namespace Domain.Interfaces
{
    using System.Linq.Expressions;

    public interface IBaseRepository<T>
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<T?> GetByAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        IQueryable<T> AsQueryable();

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
