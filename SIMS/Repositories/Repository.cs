using Microsoft.EntityFrameworkCore;
using SIMS.Data;

namespace SIMS.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly MyDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(MyDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity =  await _dbSet.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
            return entity;
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}