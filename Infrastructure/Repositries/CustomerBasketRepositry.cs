using System.Text.Json;
using Core.Entities;
using Core.interfaces;
using StackExchange.Redis;

namespace Infrastructure.Repositries
{
    public class CustomerBasketRepositry : ICustomerBasketRepositry
    {
        private readonly IDatabase _database;
        public CustomerBasketRepositry(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public Task<bool> DeleteBasketAsync(string id)
        {
            return _database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string id)
        {
            var result = await _database.StringGetAsync(id);
            if (!string.IsNullOrEmpty(result))
            {
                //return JsonSerializer.Deserialize<CustomerBasket>(result);
                //return JsonSerializer.Deserialize<CustomerBasket>(result.ToString()!);
                return JsonSerializer.Deserialize<CustomerBasket>((string)result!);
            }
            return null;
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var _basket = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(3));
            if (_basket)
            {
                return await GetBasketAsync(basket.Id);
            }
            return null;
        }
    }
}
