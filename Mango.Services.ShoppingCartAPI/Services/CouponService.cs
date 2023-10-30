using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Services.IService;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory; 
        }

        public async Task<CoupanDTO> GetCoupons(string couponcode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponcode}");
            var content = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(content);

            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CoupanDTO> (Convert.ToString(resp.Result));

            }
            return new CoupanDTO();
        }
    }
}
