
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICoupanService _couponservice;

        public CouponController(ICoupanService baseService)
        {
            _couponservice = baseService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CoupanDTO> list = new();
            ResponseDTO? response = await _couponservice.GetAllCoupanAsync();

            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CoupanDTO>>(Convert.ToString(response.Result));
            }  
            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CoupanDTO coupanDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _couponservice.CreateCoupanAsync(coupanDTO);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(coupanDTO);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDTO? response = await _couponservice.GetAllCoupanByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                CoupanDTO? model = JsonConvert.DeserializeObject<CoupanDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CoupanDTO coupanDTO)
        {
            ResponseDTO? response = await _couponservice.DeleteCoupanAsync(coupanDTO.CoupanId);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CouponIndex));
            }
            return View(coupanDTO);
        }
    }
}
