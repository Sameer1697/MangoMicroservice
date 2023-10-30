
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
            _baseService = baseService; 
        }
        public async Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = productDTO,
                Url = SD.PRODUCTAPIBASE + "/api/product",
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDTO?> DeleteProductAsync(int id)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.PRODUCTAPIBASE + "/api/product/" + id
            });
        }

        public async Task<ResponseDTO?> GetAllProductAsync()
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.PRODUCTAPIBASE + "/api/product"
            });

        }

        public async Task<ResponseDTO?> GetAllProductByIdAsync(int id)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.PRODUCTAPIBASE + "/api/product/" + id
            });
        }

        public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.PUT,
                Data = productDTO,
                Url = SD.PRODUCTAPIBASE + "/api/product",
                ContentType = SD.ContentType.MultipartFormData
            });
        }
    }
}
