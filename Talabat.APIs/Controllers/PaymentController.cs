using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Basket;
using Talabat.Core.Services.Contract.IPaymentService;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        // This is your Stripe CLI webhook secret for testing your endpoint locally.
       private const string endpointSecret = "whsec_c16caff3f0fb144e28b1d1478021288a4b20c67ab596674c9";

        public PaymentController(IPaymentService paymentService,
            IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }


        // Create Or Update EndPoint
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var CustomerBasket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);   // Now I have the items by this basketId
            if (CustomerBasket is null) return BadRequest(new ApiResponse(400, "There is a Problem With Your BasketId"));

            var MappedBasket = _mapper.Map<CustomerBasket ,CustomerBasketDto>(CustomerBasket);
            return Ok(MappedBasket);
        }

        [HttpPost("webhook")]  //POST: baseUrl/api/payment/webhook
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {

                var stripeEvent = EventUtility.ConstructEvent(json,
                     Request.Headers["Stripe-Signature"], endpointSecret);

                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;   
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentFailed)
                {
                   await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, false);  
                }

                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, true);
                }

                return Ok();
            }

            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
