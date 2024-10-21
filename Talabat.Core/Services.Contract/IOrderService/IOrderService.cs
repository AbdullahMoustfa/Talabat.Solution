using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Oreder_Aggragate;

namespace Talabat.Core.Services.Contract.IOrderService
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address ShippingAddress);

        Task<IReadOnlyList<Order>> GetOrderForSpecificUser(string buyerEmail);

        Task<Order?> GetOrderByIdForSpecificUser(string buyerEmail, int orderId);
    }
}
