using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Messages;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string createOrderTopic;
        private readonly string OrderCreatedRewardsSubscription;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _processor;
        private readonly RewardService _rewardService;
        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnnectionString");
            createOrderTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            OrderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);
            _processor = client.CreateProcessor(createOrderTopic, OrderCreatedRewardsSubscription);
            _rewardService = rewardService;  
        }

       
        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnRewardsUpdatedReceived;
            _processor.ProcessErrorAsync += OnErrorHandler;
           await _processor.StartProcessingAsync();

        }
        private Task OnErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnRewardsUpdatedReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessages? rewardmessage = JsonConvert.DeserializeObject<RewardsMessages>(body);
            try
            {
                await _rewardService.UpdateRewards(rewardmessage);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task OnRegisterEmailRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string? objectmessgae = JsonConvert.DeserializeObject<string>(body);
            try
            {
                //await _emailService.RegisterEmailandLog(objectmessgae);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task Stop()
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }
    }
}
