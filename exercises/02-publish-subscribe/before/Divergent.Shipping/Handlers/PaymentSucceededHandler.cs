using System.Threading.Tasks;
using Divergent.Finance.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Handlers
{
    public class PaymentSucceededHandler : IHandleMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<PaymentSucceededEvent>();

        public Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handled");
            return Task.CompletedTask;
        }
    }
}