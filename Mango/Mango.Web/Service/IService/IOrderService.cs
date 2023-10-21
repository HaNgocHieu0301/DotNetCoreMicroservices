using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO> CreateOrder(CartDTO cartDto);
        Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDTO> ValidateStripeSession(int orderHeaderId);
    }
}
