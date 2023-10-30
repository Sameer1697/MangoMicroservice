
using Mango.Services.RewardAPI.Messaging;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer serviceBusConsumer { get; set; }
        public static  IApplicationBuilder useAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            serviceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostapplicatonLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostapplicatonLife.ApplicationStarted.Register(OnStart);
            hostapplicatonLife.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            serviceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            serviceBusConsumer.Start();
        }
    }
}
