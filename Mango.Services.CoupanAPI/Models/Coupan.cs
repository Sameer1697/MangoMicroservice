namespace Mango.Services.CoupanAPI.Models
{
    public class Coupan
    {
        public int CoupanId { get; set; }

        public string CoupanCode { get; set; }

        public double DiscountAmount { get; set; }

        public int  MinAmount { get; set; }
    }
}
