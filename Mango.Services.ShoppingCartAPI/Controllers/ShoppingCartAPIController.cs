using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;
        private ResponseDTO responseDTO;
        private readonly IProduct _product;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public ShoppingCartAPIController(IMapper mapper, AppDbContext appDbContext,IProduct product,ICouponService couponService,IMessageBus messageBus, IConfiguration configuration)
        {
            _appDbContext = appDbContext;   
            _mapper = mapper;
            this.responseDTO = new ResponseDTO();
            _product = product; 
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cartDTO = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_appDbContext.cartHeaders.First(u => u.UserId == userId))
                };
                cartDTO.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_appDbContext.cartDetails.
                     Where(u => u.CartHeaderId == cartDTO.CartHeader.CartHeaderId));

                IEnumerable<ProductDTO> productDTOs = await _product.GetProducts();
                
                foreach(var item in cartDTO.CartDetails)
                {
                    item.Product = productDTOs.FirstOrDefault(u => u.Id == item.ProductId);
                    cartDTO.CartHeader.CartTotal += (item.Count * item.Product.Price);

                }

                if (!string.IsNullOrEmpty(cartDTO.CartHeader.CouponCode))
                {
                    CoupanDTO coupon = await _couponService.GetCoupons(cartDTO.CartHeader.CouponCode);
                    if(coupon != null && cartDTO.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cartDTO.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cartDTO.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }
                responseDTO.Result = cartDTO;

            }catch(Exception ex)
            {
                responseDTO.IsSuccess = false;
                responseDTO.Message = ex.Message;
            }
            return responseDTO;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartfromdb = await _appDbContext.cartHeaders.FirstAsync(u => u.UserId == cartDTO.CartHeader.UserId);
                cartfromdb.CouponCode = cartDTO.CartHeader.CouponCode;

                _appDbContext.cartHeaders.Update(cartfromdb);
                await _appDbContext.SaveChangesAsync();
                responseDTO.Result = true;
            }catch(Exception ex)
            {
                responseDTO.IsSuccess = false;
                responseDTO.Message = ex.ToString();
            }
            return responseDTO;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDTO cartDTO)
        {
            try
            {
                await _messageBus.PublishMessage(cartDTO, _configuration.GetValue<string>("TopicQueueName:EmailShoppingCart"));
            }
            catch (Exception ex)
            {
                responseDTO.IsSuccess = false;
                responseDTO.Message = ex.ToString();
            }
            return responseDTO;
        }


        [HttpPost("cartUpsert")]
        public async Task<ResponseDTO> cartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _appDbContext.cartHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == cartDTO.CartHeader.UserId);
                if(cartHeaderFromDb == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _appDbContext.cartHeaders.Add(cartHeader);
                    await _appDbContext.SaveChangesAsync();
                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _appDbContext.cartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _appDbContext.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _appDbContext.cartDetails.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == cartDTO.CartDetails.First().ProductId && 
                                          x.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                    if(cartDetailsFromDb == null)
                    {
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _appDbContext.cartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details

                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _appDbContext.cartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                }
                responseDTO.Result = cartDTO;
            }
            catch (Exception ex)
            {
                responseDTO.Message = ex.Message.ToString();
                responseDTO.IsSuccess = false;
            }
            return responseDTO;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody ]int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _appDbContext.cartDetails.First(u => u.CartDetailsId == cartDetailsId);
                int totalCountofCartItems = _appDbContext.cartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _appDbContext.cartDetails.Remove(cartDetails);
                if(totalCountofCartItems == 1)
                {
                    var cartHeaderFromDb = _appDbContext.cartHeaders.First(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _appDbContext.cartHeaders.Remove(cartHeaderFromDb);
                }
                await _appDbContext.SaveChangesAsync();
                responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                responseDTO.Message = ex.Message.ToString();
                responseDTO.IsSuccess = false;
            }
            return responseDTO;
        }
    }
}
