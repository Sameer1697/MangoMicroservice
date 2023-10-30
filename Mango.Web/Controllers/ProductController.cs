
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productservice;

        public ProductController(IProductService baseService)
        {
            _productservice = baseService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO> list = new();
            ResponseDTO? response = await _productservice.GetAllProductAsync();

            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }  
            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productservice.CreateProductAsync(productDTO);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(productDTO);
        }

        public async Task<IActionResult> ProductDelete(int productid)
        {
            ResponseDTO? response = await _productservice.GetAllProductByIdAsync(productid);

            if (response != null && response.IsSuccess)
            {
                ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO productDto)
        {
            ResponseDTO? response = await _productservice.DeleteProductAsync(productDto.Id);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
            return View(productDto);
        }

        public async Task<IActionResult> ProductEdit(int productid)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productservice.GetAllProductByIdAsync(productid);

                if (response != null && response.IsSuccess)
                {
                    ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                    return View(model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDTO productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productservice.UpdateProductAsync(productDto);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(productDto);
        }
    }
}
