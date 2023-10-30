using Mango.Services.ShoppingCartAPI.Models.DTO;

namespace Mango.Services.ShoppingCartAPI.Services.IService
{
    public interface IProduct
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
