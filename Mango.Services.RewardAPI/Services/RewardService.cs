
using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Messages;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardAPI.Services
{
    public class RewardService : IRewardsService
    {
        private DbContextOptions<AppDbContext> _dboptions;

        public RewardService(DbContextOptions<AppDbContext> dboptions)
        {
            _dboptions = dboptions;
        }

        public async Task UpdateRewards(RewardsMessages rewardsMessages)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessages.Orderid,
                    userId = rewardsMessages.UserId,
                    RewardsActivity = rewardsMessages.RewardsActivity,
                    RewardsDate = DateTime.Now
                };
                await using var _db = new AppDbContext(_dboptions);
                await _db.rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
               
            }catch(Exception ex)
            {
               
            }
        }
    }
}
