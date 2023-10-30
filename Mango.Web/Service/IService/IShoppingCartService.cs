
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IShoppingCartService
    {
        Task<ResponseDTO?> GetCartByUserId(string userId);
        Task<ResponseDTO?> UpsertCartasync(CartDTO cartDTO);

        Task<ResponseDTO?> RemoveCartasync(int cartDetailsId);
        Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO);
        Task<ResponseDTO?> EmailCart(CartDTO cartDTO);

    }
}
