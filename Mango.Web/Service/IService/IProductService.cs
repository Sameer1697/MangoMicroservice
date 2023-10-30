
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProductAsync();
        Task<ResponseDTO?> GetAllProductByIdAsync(int id);
        Task<ResponseDTO?> CreateProductAsync(ProductDTO coupanDTO);
        Task<ResponseDTO?> UpdateProductAsync(ProductDTO coupanDTO);
        Task<ResponseDTO?> DeleteProductAsync(int id);

    }
}
