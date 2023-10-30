

using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Services.IService
{
    public interface ICouponService
    {
        Task<CoupanDTO> GetCoupons(string couponcode);
    }
}
