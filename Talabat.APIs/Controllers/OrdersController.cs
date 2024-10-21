using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Oreder_Aggragate;
using Talabat.Core.Services.Contract.IOrderService;
using Order = Talabat.Core.Entities.Oreder_Aggragate.Order;

namespace Talabat.APIs.Controllers
{

    public class OrdersController : BaseApiController
    {

        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService, 
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        // Create Order
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost] // POST: url/api/Orders
        [Authorize]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
        
            var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);
           
            var order = await _orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);

            if (order is null) return BadRequest(new ApiResponse(400, "There is a problem with you order"));
            
            return Ok(order);
        }


        [HttpGet]  // GET: url/api/Order
        [Authorize]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderForSpecificUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);   

            var orders = await _orderService.GetOrderForSpecificUser(BuyerEmail);

            if (orders is null) return NotFound(new ApiResponse(404, "There is no Orders For This User"));
           
            var MappedOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders); 
            return Ok(MappedOrders);
        }


        [HttpGet("{id}")]  // GET: url/api/Order
        [Authorize]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForSpecificUser(int id)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var order = await _orderService.GetOrderByIdForSpecificUser(BuyerEmail, id);

            if (order is null) return NotFound(new ApiResponse(404, $"There is no Order With Id = {id} For This User "));
            
            var MappedOrder = _mapper.Map<Order, OrderToReturnDto>(order);  
            return Ok(MappedOrder);   
        }


        [HttpGet("DeliveryMethods")] // GET: url/api/Orders/DeliveryMethods
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            return Ok(DeliveryMethods);
        }



    }
}
