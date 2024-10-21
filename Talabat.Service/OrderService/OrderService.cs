using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Entities.Oreder_Aggragate;
using Talabat.Core.Entities.Product;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract.IOrderService;
using Talabat.Core.Services.Contract.IPaymentService;
using Talabat.Core.Specifications.OrderSpecs;
using Talabat.Repository.Generic_Repository;

namespace Talabat.Service.OrderService
{
    public class OrderService : IOrderService
    {
        #region Ask CLR For Creating Objects From Classes Which Implement Those Interfaces
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        //private readonly IGenericRepositories<Product> _productRepo;
        //private readonly IGenericRepositories<DeliveryMethod> _deliveryMethodRepo;
        //private readonly IGenericRepositories<Order> _orderRepo;

        public OrderService(IBasketRepository basketRepo
                        , IUnitOfWork unitOfWork
                        , IPaymentService paymentService
                        // , IGenericRepositories<Product> productRepo
                        // , IGenericRepositories<DeliveryMethod> deliveryMethodRepo
                        // , IGenericRepositories<Order> orderRepo
                           )
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            // _productRepo = productRepo;
            // _deliveryMethodRepo = deliveryMethodRepo;
            // _orderRepo = orderRepo;
        } 
        #endregion

        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            
            // 1.Get Basket From Baskets Repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();

            if(basket?.Items?.Count > 0)
            {
                foreach(var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id); // كده انا جبت البرودكت

                    var ProductItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(ProductItemOrdered, product.Price, item.Quantity);
                
                    orderItems.Add(orderItem);  
                }
            }

            // 3. Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Quantity * item.Price);  

            // 4. Get Delivery Method From DeliveryMethods Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 5. Create Order
            var spec = new orderWithPaymentIntentSpecification(basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

            // 6. Add Order Locally
            await _unitOfWork.Repository<Order>().AddAsync(order);

            // 7. Save To Database [TODO]
            var result = await _unitOfWork.CompleteAsync();   

            if(result<=0) return null;

            return order;
        }


        public async Task<IReadOnlyList<Order>> GetOrderForSpecificUser(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);

            var order = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return order;
        }


        public async Task<Order?> GetOrderByIdForSpecificUser(string buyerEmail, int orderId)
        {
            var spec = new OrderSpecifications(buyerEmail, orderId);

            var orders = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            return orders;
        }

       
    }
}
