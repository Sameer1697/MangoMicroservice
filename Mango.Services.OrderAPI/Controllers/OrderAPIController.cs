using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Services;
using Mango.Services.OrderAPI.Services.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Collections.Generic;
using Stripe.Checkout;
using Mango.MessageBus;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProduct _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public OrderAPIController(IMapper mapper, AppDbContext db, IProduct productService, IConfiguration configuration, IMessageBus messageBus)
        {
            _response = new ResponseDTO();
            _mapper = mapper;
            _db = db;
            _productService = productService;   
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseDTO> Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objlist;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objlist = _db.OrderHeaders.Include(u => u.OrderDetails).OrderByDescending(u => u.orderHeaderId).ToList();
                }
                else
                {
                    objlist = _db.OrderHeaders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.orderHeaderId).ToList();
                }
                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(objlist);
            }
            catch(Exception ex)
            {
                _response.Message = ex.ToString();
                _response.IsSuccess = false;
            }

            return _response;
        }

        [Authorize]
        [HttpGet("GetOrder/{orderid:int}")]
        public async Task<ResponseDTO> Get(int orderid)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails).First(u => u.orderHeaderId == orderid);
                _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.Message = ex.ToString();
                _response.IsSuccess = false;
            }

            return _response;
        }


        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.Ordertime = DateTime.Now;
                orderHeaderDTO.status = SD.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(cartDTO.CartDetails);

                OrderHeader orderCreated =  _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDTO.orderHeaderId = orderCreated.orderHeaderId;

               
                _response.Result = orderHeaderDTO;

            }catch(Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApproveUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    
                };

                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon = stripeRequestDTO.orderHeaderDTO.CouponCode
                    }
                };

                foreach(var item in stripeRequestDTO.orderHeaderDTO.OrderDetails)
                {
                    var sesseionlineitem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sesseionlineitem);
                }

                if(stripeRequestDTO.orderHeaderDTO.Discount > 0)
                {
                    options.Discounts = discountObj;
                }
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.orderHeaderId == stripeRequestDTO.orderHeaderDTO.orderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDTO;
            }
            catch(Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }

        [Authorize]
        [HttpPost("ValidateStripSession")]
        public async Task<ResponseDTO> ValidateStripSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.orderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);
                var paymentinterntservice = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentinterntservice.Get(session.PaymentIntentId);
                if(paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.status = SD.Status_Approved;
                    _db.SaveChanges();
                    RewardsDTO rewardsDTO = new RewardsDTO()
                    {
                        Orderid = orderHeader.orderHeaderId,
                        RewardsActivity = Convert.ToInt32(orderHeader.OrderTotal),
                        UserId = orderHeader.UserId
                    };
                    string topicname = _configuration.GetValue<string>("TopicQueueName:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDTO,topicname);
                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;

        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderid:int}")]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderid, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.orderHeaderId == orderid);
                if(orderHeader != null)
                {
                    if(newStatus == SD.Status_Cancelled)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId,

                        };
                        var service = new RefundService();
                        Refund refund = service.Create(options);
                      
                    }
                    orderHeader.status = newStatus;
                    await _db.SaveChangesAsync();
                }
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }
    }
}
