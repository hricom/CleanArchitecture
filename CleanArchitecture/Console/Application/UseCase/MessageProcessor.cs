using Console.Application.Boundaries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Application.UseCase
{
    public class MessageProcessor ( ILogger<MessageProcessor> logger ) : IMessageProcessor
    {       
        private readonly ILogger<MessageProcessor> _logger = logger;

        public async Task ProcessAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Processing message: {message}");

                // Simulate message processing
                await Task.Delay(500, cancellationToken);

                // Example: Throw an exception for invalid messages
                if (message.Contains("error"))
                {
                    throw new InvalidOperationException("Invalid message content.");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error processing message.");
                throw; // Re-throw the exception to be handled by the subscriber service
            }
        }
    }
}
