using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console.Application.Boundaries
{
    public interface IMessageProcessor
    {
        Task ProcessAsync(string message, CancellationToken cancellationToken);
    }
}
