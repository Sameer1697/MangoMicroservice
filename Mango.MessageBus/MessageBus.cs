using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionstring = "Endpoint=sb://mangowebsameer.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LJLXesVOPqqarhJE5wtl2Y85Rn733t2Ii+ASbDvvlqg=";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connectionstring);

            ServiceBusSender sender = client.CreateSender(topic_queue_name);
            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalmessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalmessage);
            await client.DisposeAsync();
        }
    }
}
