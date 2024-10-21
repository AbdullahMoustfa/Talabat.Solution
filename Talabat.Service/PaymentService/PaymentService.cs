using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Oreder_Aggragate;
using Talabat.Core.Entities.Product;
using Talabat.Core.Services.Contract.IPaymentService;
using Talabat.Core.Specifications.OrderSpecs;
using Talabat.Repository.Generic_Repository;
using Product = Talabat.Core.Entities.Product.Product;

namespace Talabat.Service.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepo;

        public PaymentService(IConfiguration configuration,
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
            //1. Get Secret key
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];

            // 2.Get Basket
            var Basket = await _basketRepo.GetBasketAsync(BasketId);
            if (Basket is null) return null;  // In Case Basket Null

            var ShippingPrice = 0M;  // Decimal
            if (Basket.DeliveryMethodId.HasValue)   // In Case Basket is not null and the DM Id Has Value
            {
              var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
                ShippingPrice = DeliveryMethod.Cost;
            } 

            // 3.Total = Subtotal + ShippingPrice (DM Cost)
               if(Basket.Items.Count > 0)
            {
                foreach(var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);    

                    if(item.Price != Product.Price)
                        item.Price = Product.Price; 
                }
            }

            // Now I Will Get the Subtotal For Items Which Existed In Basket
            var Subtotal = Basket.Items.Sum(item => item.Price * item.Quantity);    


            // 4.Create Payment Intent
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if(string.IsNullOrEmpty(Basket.PaymentIntentId))  // Create
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount =(long) (Subtotal*100 + ShippingPrice*100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card"}
                };
             paymentIntent =  await service.CreateAsync(Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = Basket.ClientSecret;  
            }
            else  // Update
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long) (Subtotal*100 + ShippingPrice *100)
                };
                paymentIntent = await service.UpdateAsync(BasketId, Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = Basket.ClientSecret;
            }

            await _basketRepo.UpdateBasketAsync(Basket);
            return Basket;  
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag)
        {
            var spec = new orderWithPaymentIntentSpecification(PaymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (flag) 
            {
                order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                order.Status = OrderStatus.PaymentFailed;
            }

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();    

            return order;   
        }
    }
}
