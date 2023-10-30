using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDTO,
                Url = SD.AUTHAPIBASE + "/api/auth/AssignRole"
            });
        }

        public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = SD.AUTHAPIBASE + "/api/auth/login"
            });
        }

        public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDTO)
        {
            return await _baseService.sendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDTO,
                Url = SD.AUTHAPIBASE + "/api/auth/register"
            });
        }
    }
}
