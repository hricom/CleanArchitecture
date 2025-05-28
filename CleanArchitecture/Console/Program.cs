
namespace Console
{

    #region using
    using Console.Application.Boundaries;
    using Console.Application.UseCase;
    using Console.Infrastructure.Receivers.BackgroundServices;
    using Google.Cloud.PubSub.V1;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    #endregion

    internal class Program
    {
        static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {

                // Register SubscriberClient as a singleton
                services.AddSingleton(provider =>
                {
                    // Get configuration (you can also use IOptions<PubSubOptions>)
                    var configuration = provider.GetRequiredService<IConfiguration>();

                    // Read settings from configuration
                    string? projectId = configuration["PubSub:ProjectId"];
                    string? subscriptionId = configuration["PubSub:SubscriptionId"];

                    // Create and return the SubscriberClient
                    SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionId);
                    return SubscriberClient.Create(subscriptionName);
                });

                services.AddHostedService<PubSubSubscriberService>(); // Infrastructure layer
                services.AddScoped<IMessageProcessor, MessageProcessor>(); // Application layer
            });
    }
}