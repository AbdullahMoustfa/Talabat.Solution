using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository.Generic_Repository
{
    public class GenericRepository<T> : IGenericRepositories<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region WithoutSpec

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            // if(typeof(T) == typeof(Product))
            //     return (IReadOnlyList<T>)await _dbContext.Set<Product>().Skip(20).Take(10).OrderBy(P => P.Name).Include(P => P.Brand).Include(P => P.Category).ToListAsync();

            return await _dbContext.Set<T>().ToListAsync();
        
        }

        public async Task<T?> GetByIdAsync(int id)
        {
          //  if (typeof(T) == typeof(Product))
          //      return await _dbContext.Set<Product>().Where(P => P.Id == id).Include(P => P.Brand).Include(P => P.Category).FirstOrDefaultAsync() as T;

            return await _dbContext.Set<T>().FindAsync(id);
        }
        #endregion

        #region WithSpec

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).ToListAsync();
            // _dbContext.Product.Where(P => P.BrandId == 2 && true).OrderBy(P => P.Name).Include(P => P.Brand).Include(P => P.Category).ToListAsync();

        }


        public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }

        public IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public async Task<int> GetCountWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }


        #endregion

        public async Task AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
        }


    }
}
