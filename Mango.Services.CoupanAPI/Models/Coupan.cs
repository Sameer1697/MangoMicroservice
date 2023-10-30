using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CoupanAPI.Models
{
    public class Coupan
    {
        [Key]
        public int CoupanId { get; set; }

        [Required]
        public string CoupanCode { get; set; }

        [Required]
        public double DiscountAmount { get; set; }

        public int  MinAmount { get; set; }

    }
}
