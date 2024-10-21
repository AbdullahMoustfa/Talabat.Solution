using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Basket;

namespace Talabat.Repository.Generic_Repository
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasketAsync(string basketId);

        ////CustomerBasket basket => because i need to create also.
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket); 
        Task<bool> DeleteBasketAsync(string basketId);
    }
}
