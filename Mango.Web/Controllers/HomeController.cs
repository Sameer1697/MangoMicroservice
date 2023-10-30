using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        public HomeController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService; 
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDTO> list = new();
            ResponseDTO? response = await _productService.GetAllProductAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        [Authorize]
        public async Task<IActionResult> Details(int productid)
        {
            ProductDTO? model = new();
            ResponseDTO? response = await _productService.GetAllProductByIdAsync(productid);

            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> Details(ProductDTO productDTO)
        {
            CartDTO cartDTO = new CartDTO()
            {
                CartHeader = new CartHeaderDTO
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDTO cartDetailsDTO = new CartDetailsDTO()
            {
                Count = productDTO.Count,
                ProductId = productDTO.Id,
            };

            List<CartDetailsDTO> cartDetailsDTOs = new() { cartDetailsDTO };
            cartDTO.CartDetails = cartDetailsDTOs;

            ResponseDTO? responseDTO = await _shoppingCartService.UpsertCartasync(cartDTO);

            if(responseDTO != null && responseDTO.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDTO);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}