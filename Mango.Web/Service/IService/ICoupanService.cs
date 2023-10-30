
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICoupanService
    {
        Task<ResponseDTO?> GetCoupanAsync(string coupanCode);
        Task<ResponseDTO?> GetAllCoupanAsync();
        Task<ResponseDTO?> GetAllCoupanByIdAsync(int id);
        Task<ResponseDTO?> CreateCoupanAsync(CoupanDTO coupanDTO);
        Task<ResponseDTO?> UpdateCoupanAsync(CoupanDTO coupanDTO);
        Task<ResponseDTO?> DeleteCoupanAsync(int id);

    }
}
