using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Oreder_Aggragate;

namespace Talabat.Core.Specifications.OrderSpecs
{
    public class orderWithPaymentIntentSpecification : BaseSpecifications<Order>
    {
        public orderWithPaymentIntentSpecification(string PaymentIntentId):base(O => O.PaymentIntentId == PaymentIntentId)
        {
            
        }
    }
}
