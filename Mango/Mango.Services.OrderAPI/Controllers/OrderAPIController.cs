using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.FinancialConnections;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductService _productService;
        public OrderAPIController(IMapper mapper, AppDbContext db, IProductService productService)
        {
            _response = new ResponseDTO();
            _mapper = mapper;
            _db = db;
            _productService = productService;
        }

        [Authorize]
        [HttpPost("TestOrder")]
        public async Task<ActionResult> TestOrder(int orderId)
        {
            return Ok("test oke");
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDto)
        {
            try
            {
                OrderHeaderDTO orderHeaderDto = _mapper.Map<OrderHeaderDTO>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDto.CartDetails);

                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDto)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var DiscountsObj = new List<SessionDiscountOptions>
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price) * 100, //20.99$ -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            }
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);
                if (paymentIntent.Status == "succeeded")
                {
                    //then payment was successful
                    orderHeader.PaymentIntendId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
