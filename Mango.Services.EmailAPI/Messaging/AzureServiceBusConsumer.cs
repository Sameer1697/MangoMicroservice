using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Messages;
using Mango.Services.EmailAPI.Models.DTO;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailQueue;
        private readonly string registeruserQueue;
        private readonly IConfiguration _configuration;
        private readonly string orderCreated_Topic;
        private readonly string ordercreated_email_subscription;
        private ServiceBusProcessor _emailOrdrProcessor;
        private ServiceBusProcessor _processor;
        private ServiceBusProcessor _registeremailprocessor;
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnnectionString");
            emailQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart");
            registeruserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUsernamequeue");
            orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            ordercreated_email_subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");
            var client = new ServiceBusClient(serviceBusConnectionString);
            _processor = client.CreateProcessor(emailQueue);
            _registeremailprocessor = client.CreateProcessor(registeruserQueue);
            _emailOrdrProcessor = client.CreateProcessor(orderCreated_Topic, ordercreated_email_subscription);
            _emailService = emailService;  

        }

       
        public async Task Start()
        {
            _processor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _processor.ProcessErrorAsync += OnErrorHandler;
           await _processor.StartProcessingAsync();

            _registeremailprocessor.ProcessMessageAsync += OnRegisterEmailRequestReceived;
            _registeremailprocessor.ProcessErrorAsync += OnErrorHandler;
            await _registeremailprocessor.StartProcessingAsync();

            _emailOrdrProcessor.ProcessMessageAsync += OnOrdrPlacedRequestReceived;
            _emailOrdrProcessor.ProcessErrorAsync += OnErrorHandler;
            await _emailOrdrProcessor.StartProcessingAsync();
            
        }

      

        private Task OnErrorHandler(ProcessErrorEventArgs arg)
        {
            Console.WriteLine(arg.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDTO? cartDTO = JsonConvert.DeserializeObject<CartDTO>(body);
            try
            {
                await _emailService.EamilCartLog(cartDTO);
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
                await _emailService.RegisterEmailandLog(objectmessgae);
                await arg.CompleteMessageAsync(arg.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnOrdrPlacedRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessages? objectmessgae = JsonConvert.DeserializeObject<RewardsMessages>(body);
            try
            {
                await _emailService.OrderCompletedMessage(objectmessgae);
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

            await _registeremailprocessor.StopProcessingAsync();
            await _registeremailprocessor.DisposeAsync();

            await _emailOrdrProcessor.StopProcessingAsync();
            await _emailOrdrProcessor.DisposeAsync();
        }
    }
}
