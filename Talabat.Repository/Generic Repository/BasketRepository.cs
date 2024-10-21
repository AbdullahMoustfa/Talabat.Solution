using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities.Basket;

namespace Talabat.Repository.Generic_Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer Redis)
        {
            _database = Redis.GetDatabase();
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var basket = await _database.StringGetAsync(basketId);

            return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }


        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var Json_Serialize = JsonSerializer.Serialize<CustomerBasket>(basket);

            var createOrUpdate = await _database.StringSetAsync(basket.Id, Json_Serialize, TimeSpan.FromDays(30));

            if (!createOrUpdate) return null;

            return await GetBasketAsync(basket.Id);
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }
    }
}
