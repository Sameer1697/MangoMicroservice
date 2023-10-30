using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI.Services.IService
{
    public interface IProduct
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
