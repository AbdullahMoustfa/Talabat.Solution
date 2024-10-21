using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.Oreder_Aggragate;

namespace Talabat.APIs.DTOs
{
    public class OrderDto
    {
        [Required]
        public string BasketId { get; set; }

        [Required]
        public int DeliveryMethodId { get; set; }

        [Required]
        public AddressDto shipToAddress { get; set; }

    }
}
