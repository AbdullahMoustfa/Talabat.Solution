using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Oreder_Aggragate;

namespace Talabat.Core.Services.Contract.IPaymentService
{
    public interface IPaymentService 
    {
        // Fun To Create Or Update Payment Intent
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId);

        Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag);
    }
}
