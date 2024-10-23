using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract.IResponseCacheService;

namespace Talabat.Service.ResponseCacheService
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CacheResponseAsync(string key, object response, TimeSpan timeToLive)
        {
            if(response is null) return;
 
            var serializedOptions = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};  
            var SerializedResponse = JsonSerializer.Serialize(response, serializedOptions);
            await _database.StringSetAsync(key, SerializedResponse, timeToLive); 
        }

        public async Task<string?> GetCachedResponseAsync(string key)
        {
            var response = await _database.StringGetAsync(key); 
            if(response.IsNullOrEmpty) return null;
            
            return response;    
        }
    }
}
