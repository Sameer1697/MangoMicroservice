using Mango.Services.EmailAPI.Messages;
using Mango.Services.EmailAPI.Models.DTO;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task  EamilCartLog(CartDTO cartDTO);    

        Task RegisterEmailandLog(string email);

        Task OrderCompletedMessage(RewardsMessages rewardsMessages);
    }
}
