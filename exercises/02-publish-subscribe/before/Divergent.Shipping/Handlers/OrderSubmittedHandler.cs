using System.Threading.Tasks;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Handlers
{
    public class OrderSubmittedHandler : IHandleMessages<OrderSubmittedEvent>
    {
        private static ILog Log = LogManager.GetLogger<OrderSubmittedHandler>();

        public Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handled");
            return Task.CompletedTask;
        }
    }
}