using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TwitterAPI.Domain.Abstractions;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository<T>(ApplicationDbContext dbContext) : IBaseRepository<T>
        where T : Entity
    {
        protected ApplicationDbContext DbContext { get; } = dbContext;

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().FindAsync([id], cancellationToken);
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await this.DbContext.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            this.DbContext.Update(entity);
        }

        public async Task<T?> GetByAsync(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<T> AsQueryable()
        {
            return this.DbContext.Set<T>().AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await this.DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
