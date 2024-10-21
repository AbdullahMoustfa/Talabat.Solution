using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Oreder_Aggragate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class OrderSpecifications : BaseSpecifications<Order>
    {
        public OrderSpecifications(string email)
            :base(O=>O.BuyerEmail == email)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O=>O.Items);

            AddOrderByDesc(O=>O.OrderDate);
        }

        public OrderSpecifications(string buyerEmail, int orderId)
            :base(O => O.BuyerEmail == buyerEmail && O.Id == orderId)

        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }
    }
}
