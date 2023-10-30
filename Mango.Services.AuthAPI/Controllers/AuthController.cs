using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        protected ResponseDTO _response;

        public AuthController(IAuthService authService,IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
            _response = new();   
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicQueueName:RegisterUsernamequeue"));
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginRespons = await _authService.Login(model);

            if(loginRespons.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or Password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginRespons;    
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
        {
            var assignroleSuccesful = await _authService.AssignRole(model.Email, model.Role.ToUpper());

            if (!assignroleSuccesful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encounter";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
