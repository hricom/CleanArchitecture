
namespace Console.Infrastructure.Receivers.BackgroundServices
{
    using Console.Application.Boundaries;
    using Google.Apis.Auth.OAuth2;
    using Google.Cloud.Firestore;
    #region using
    using Google.Cloud.PubSub.V1;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    public class PubSubSubscriberService(SubscriberClient subscriberClient, ILogger<PubSubSubscriberService> logger,
        IMessageProcessor messageProcessor) : BackgroundService
    {
        private readonly SubscriberClient _subscriberClient = subscriberClient;
        private readonly ILogger<PubSubSubscriberService> _logger = logger;
        private readonly IMessageProcessor _messageProcessor = messageProcessor;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Pub/Sub message processing");
            try
            {

                await _subscriberClient.StartAsync(async (message, cancel) =>
                {
                    try
                    {

                        string textIn = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                        _logger.LogInformation($"Received message: {textIn}");

                        // Process the message here
                        // Process the message
                        await _messageProcessor.ProcessAsync(message: textIn, cancellationToken: stoppingToken);

                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(ex, $"Error processing message {message.MessageId}.");
                        // You can Nack the message to retry later
                        return SubscriberClient.Reply.Nack;
                    }
                });

                _logger.LogInformation("Pub/Sub Subscriber Service is running.");

                // Wait until the service is stopped
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }

            }
            catch (Exception ex)
            {

                _logger.LogCritical(ex, "Pub/Sub Subscriber Service encountered a fatal error.");
            }
            finally
            {

                _logger.LogInformation("Pub/Sub Subscriber Service is stopping.");
                // Stop the subscriber
                await StopAsync(stoppingToken:  stoppingToken);

            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stopping Pub/Sub message processing");
            await _subscriberClient.StopAsync(stoppingToken);
            await base.StopAsync(stoppingToken);
        }
    }
}
