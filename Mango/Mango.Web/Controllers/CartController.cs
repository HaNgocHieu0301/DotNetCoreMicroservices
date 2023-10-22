using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;
        private IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
        public async Task<CartDTO> LoadCartDtoBasedOnLoggedInUser()
        {
            //register Sub ~ Id in JwtTokenGenerator
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;

            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDTO cartDto = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDTO();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            ResponseDTO? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDto)
        {

            CartDTO cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault().Value;
            var response = await _cartService.EmailCart(cart);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Email will be sent in short time";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        #region Checkout

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDto)
        {
            CartDTO cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            var response = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                //get stripe session and redirect to stripe to place order

                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDTO stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                StripeRequestDTO stripeReponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>
                    (Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeReponseResult.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            return View();
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO> (Convert.ToString(response.Result));
                if (orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to seome error page based on status but now we only focus on mcsv, so ignore it
            return View(orderId);
        }

        #endregion

        #region Coupon Apply - Remove
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDto)
        {
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        #endregion
    }
}
