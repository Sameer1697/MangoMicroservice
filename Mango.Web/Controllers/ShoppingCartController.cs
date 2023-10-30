
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;

        public ShoppingCartController(IShoppingCartService baseService, IOrderService orderService)
        {
            _shoppingCartService = baseService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> cartIndex()
        {
            return View(await LoadCartbasedonLoggedInUser());
        }
        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartbasedonLoggedInUser());
        }

        [Authorize]
        [HttpPost]
        [ActionName("CheckOut")]
        public async Task<IActionResult> CheckOut(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartbasedonLoggedInUser();
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Firstname = cartDTO.CartHeader.Firstname;
            cart.CartHeader.LastName = cartDTO.CartHeader.LastName;

            var resonse = await _orderService.CreateOrder(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(resonse.Result));

            if(resonse != null && resonse.IsSuccess)
            {
                var domaain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApproveUrl = domaain + "ShoppingCart/Confirmation?orderId=" + orderHeaderDTO.orderHeaderId,
                    CancelUrl = domaain + "ShoppingCart/CheckOut",
                    orderHeaderDTO = orderHeaderDTO
                };
                var stripresponse = await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripresponse.Result));
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);
                return new StatusCodeResult(303);
            }

            return View();
        }

        
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if(orderHeaderDTO.status == SD.Status_Approved)
                {
                    return View(orderId);
                }

            }
            return View(orderId);
        }

        private async Task<CartDTO> LoadCartbasedonLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.GetCartByUserId(userId);
            if(response != null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDTO;
            }
            return new CartDTO();

        }

        public async Task<IActionResult> Remove(int cartdetailid)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.RemoveCartasync(cartdetailid);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(cartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO  cartDTO)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(cartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartbasedonLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.EmailCart(cart);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(cartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = "";
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _shoppingCartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(cartIndex));
            }
            return View();
        }

    }
}
