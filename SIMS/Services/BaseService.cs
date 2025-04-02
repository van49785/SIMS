using SIMS.Repositories;
using SIMS.Services.Interfaces;

namespace SIMS.Services
{
    public class BaseService<T> : IService<T> where T : class
    {
            protected readonly IRepository<T> _repository;

            public BaseService(IRepository<T> repository)
            {
                _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            }

            public async Task<IEnumerable<T>> GetAllAsync()
            {
                return await _repository.GetAllAsync();
            }

            public async Task<T> GetByIdAsync(int id)
            {
                return await _repository.GetByIdAsync(id);
            }

            public async Task AddAsync(T entity)
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));
                await _repository.AddAsync(entity);
            }

            public async Task UpdateAsync(T entity)
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));
                await _repository.UpdateAsync(entity);
            }

            public async Task DeleteAsync(int id)
            {
                await _repository.DeleteAsync(id);
            }

            public async Task SaveAsync()
            {
                await _repository.SaveAsync();
            }
        }
    }

