using Talabat.Core.Entities.Oreder_Aggragate;

namespace Talabat.APIs.DTOs
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } 
        public string Status { get; set; }  // Handled in Configuration
        public Address ShippingAddress { get; set; }

        //public int DeliveryMethodId { get; set; } // FK
        public string DeliveryMethod { get; set; }  // Name
        public string DeliveryMethodCost { get; set; } // Cost

        public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>(); // Navigational Property[M]
      
        public decimal SubTotal { get; set; } // Quantity * Price Of Product
        public decimal Total { get; set; }

        public string PaymentIntentId { get; set; }
    }
}
