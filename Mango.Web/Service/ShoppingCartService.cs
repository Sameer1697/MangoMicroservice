
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;

        public ShoppingCartService(IBaseService baseService)
        {
            _baseService = baseService; 
        }

        public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.SHOPPINGCARTAPIBASE + "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseDTO?> EmailCart(CartDTO cartDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.SHOPPINGCARTAPIBASE + "/api/cart/EmailCartRequest"
            });
        }

        public async Task<ResponseDTO?> GetCartByUserId(string userId)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.SHOPPINGCARTAPIBASE + "/api/cart/GetCart/" + userId
            });
        }

        public async Task<ResponseDTO?> RemoveCartasync(int cartDetailsId)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDetailsId,
                Url = SD.SHOPPINGCARTAPIBASE + "/api/cart/RemoveCart"
            });
        }
       
        public async Task<ResponseDTO?> UpsertCartasync(CartDTO cartDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.SHOPPINGCARTAPIBASE + "/api/cart/cartUpsert"
            });
        }
    }
}
