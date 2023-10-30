
using Mango.Services.RewardAPI.Messages;

namespace Mango.Services.RewardAPI.Services
{
    public interface IRewardsService
    {
        Task  UpdateRewards(RewardsMessages rewardsMessages);    

    }
}
