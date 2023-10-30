namespace Mango.Services.OrderAPI.Models.DTO
{
    public class OrderHeaderDTO
    {
        public int orderHeaderId { get; set; }

        public string? UserId { get; set; }

        public string? CouponCode { get; set; }


        public double Discount { get; set; }

        public double OrderTotal { get; set; }


        public string? Firstname { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public DateTime? Ordertime { get; set; }

        public string? status { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? StripeSessionId { get; set; }

        public IEnumerable<OrderDetailDTO> OrderDetails { get; set; }
    }
}
