using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Messages;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dboptions;

        public EmailService(DbContextOptions<AppDbContext> dboptions)
        {
            _dboptions = dboptions;
        }

        public async Task EamilCartLog(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine("<br/> Cart Email Requested ");
            message.AppendLine("<br />Total" + cartDTO.CartHeader.CartTotal);
            message.AppendLine("<br />");
            message.AppendLine("<ul>");
            foreach(var item in cartDTO.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " , " + item.Count);
                message.AppendLine("</li>");

            }
            message.AppendLine("</ul>");
            await LogandEmail(message.ToString(), cartDTO.CartHeader.Email);
        }

        public async Task OrderCompletedMessage(RewardsMessages rewardsMessages)
        {
            string message = "order is completed" + rewardsMessages.Orderid;
            await LogandEmail(message, "dotnetmaster@mgail.com");
        }

        public async Task RegisterEmailandLog(string email)
        {
            string message = "user Registeration Successful" + email;
            await LogandEmail(message, "dotnetmaster@mgail.com");
        }

        private async Task<bool> LogandEmail(string message, string email)
        {
            try
            {
                EmailLogger emaillog = new()
                {
                    Email = email,
                    Message = message,
                    DateTime = DateTime.Now
                };
                await using var _db = new AppDbContext(_dboptions);
                await _db.EmailLoggers.AddAsync(emaillog);
                await _db.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}
