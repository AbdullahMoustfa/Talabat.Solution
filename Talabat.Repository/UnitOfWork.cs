using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data;
using Talabat.Repository.Generic_Repository;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        private Hashtable _repository;

        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            _repository = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        => await _dbContext.SaveChangesAsync();


        public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();

        public IGenericRepositories<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            // Use HashTable to Store el Entity + Object اللي بعمله منها


            var key = typeof(TEntity).Name; // Order, Product

            if (!_repository.ContainsKey(key))
            {
                var repository = new GenericRepository<TEntity>(_dbContext);

                _repository.Add(key, repository); // Now I Stored el Entity + Object اللي بعمله منها
                                                  // Like مثلا Product + new GenericRepository<Product>(_dbContext);  
            }


            return _repository[key] as IGenericRepositories<TEntity>;
        }

    }
}
