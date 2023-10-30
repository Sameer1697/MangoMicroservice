
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;

        public OrderService(IBaseService baseService)
        {
            _baseService = baseService; 
        }
        public async Task<ResponseDTO?> CreateOrder(CartDTO cartDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.ORDERAPIBASE + "/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDTO,
                Url = SD.ORDERAPIBASE + "/api/order/CreateStripSession"
            });
        }

        public async Task<ResponseDTO?> GetAllOrder(string? userId)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ORDERAPIBASE + "/api/order/GetOrders/" + userId
            });
        }

        public async Task<ResponseDTO?> GetByOrderId(int orderHeaderid)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ORDERAPIBASE + "/api/order/GetOrder/" + orderHeaderid
            });
        }

        public async Task<ResponseDTO?> UpdateOrderStatus(int orderHeaderid, string status)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = status,
                Url = SD.ORDERAPIBASE + "/api/order/UpdateOrderStatus/" + orderHeaderid
            });
        }

        public async Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.ORDERAPIBASE + "/api/order/ValidateStripSession"
            });
        }
    }
}
