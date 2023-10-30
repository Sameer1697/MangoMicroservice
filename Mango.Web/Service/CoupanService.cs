
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CoupanService : ICoupanService
    {
        private readonly IBaseService _baseService;

        public CoupanService(IBaseService baseService)
        {
            _baseService = baseService; 
        }
        public async Task<ResponseDTO?> CreateCoupanAsync(CoupanDTO coupanDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = coupanDTO,
                Url = SD.COUPANAPIBASE + "/api/coupon"
            });
        }

        public async Task<ResponseDTO?> DeleteCoupanAsync(int id)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.COUPANAPIBASE + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDTO?> GetAllCoupanAsync()
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.COUPANAPIBASE + "/api/coupon"
            });

        }

        public async Task<ResponseDTO?> GetAllCoupanByIdAsync(int id)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.COUPANAPIBASE + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDTO?> GetCoupanAsync(string coupanCode)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.COUPANAPIBASE + "/api/coupon/GetByCode/" + coupanCode
            });
        }

        public async Task<ResponseDTO?> UpdateCoupanAsync(CoupanDTO coupanDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.PUT,
                Data = coupanDTO,
                Url = SD.COUPANAPIBASE + "/api/coupon"
            });
        }
    }
}
