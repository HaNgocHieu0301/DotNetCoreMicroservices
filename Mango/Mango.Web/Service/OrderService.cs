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
        public async Task<ResponseDTO> TestOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = orderId,
                Url = SD.OrderAPIBase + "/api/order/TestOrder"
            });
        }

        public async Task<ResponseDTO> CreateOrder(CartDTO cartDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDTO,
                Url = SD.CouponAPIBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDTO> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.CouponAPIBase + "/api/order/ValidateStripeSession"
            });
        }
    }
}
