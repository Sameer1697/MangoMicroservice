using AutoMapper;
using Mango.Services.CoupanAPI.Data;
using Mango.Services.CoupanAPI.Models;
using Mango.Services.CoupanAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Stripe;

namespace Mango.Services.CoupanAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _responseDTO;
        private IMapper _mapper;
        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _responseDTO = new ResponseDTO();
            _mapper = mapper;  
        }

        [HttpGet]   
        public ResponseDTO Get()
        {
            try
            {
                IEnumerable<Coupan> objList = _db.Coupons.ToList();
                _responseDTO.Result =_mapper.Map<IEnumerable<CoupanDTO>>(objList).ToList();
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }



        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                Coupan objList = _db.Coupons.First(u => u.CoupanId == id);
                _responseDTO.Result = _mapper.Map<CoupanDTO>(objList);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDTO GetByCode(string code)
        {
            try
            {
                Coupan objList = _db.Coupons.FirstOrDefault(u => u.CoupanCode.ToLower() == code.ToLower());
                if(objList == null)
                {
                    _responseDTO.IsSuccess = false;
                }
                _responseDTO.Result = _mapper.Map<CoupanDTO>(objList);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDTO Post([FromBody] CoupanDTO coupanDTO)
        {
            try
            {
                Coupan obj = _mapper.Map<Coupan>(coupanDTO);
                _db.Coupons.Add(obj);
                _db.SaveChanges();


                var options = new CouponCreateOptions
                {
                    AmountOff = (long)(coupanDTO.DiscountAmount * 100),
                    Name = coupanDTO.CoupanCode,
                    Currency = "usd",
                    Id = coupanDTO.CoupanCode
                  
                };
                var service = new CouponService();
                service.Create(options);

                _responseDTO.Result = _mapper.Map<CoupanDTO>(obj);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Put([FromBody] CoupanDTO coupanDTO)
        {
            try
            {
                Coupan obj = _mapper.Map<Coupan>(coupanDTO);
                _db.Coupons.Update(obj);
                _db.SaveChanges();

                _responseDTO.Result = _mapper.Map<CoupanDTO>(obj);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDTO Delete(int id)
        {
            try
            {
                Coupan obj = _db.Coupons.First(u => u.CoupanId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();

                var service = new CouponService();
                service.Delete(obj.CoupanCode);
                //_responseDTO.Result = _mapper.Map<CoupanDTO>(obj);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }

    }
}
