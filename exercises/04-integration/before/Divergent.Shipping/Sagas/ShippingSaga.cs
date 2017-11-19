﻿using System.Linq;
using System.Threading.Tasks;
using Divergent.Finance.Messages.Events;
using Divergent.ITOps.Messages.Commands;
using Divergent.Sales.Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Shipping.Sagas
{
    class ShippingSaga : Saga<ShippingSagaData>,
        IAmStartedByMessages<OrderSubmittedEvent>,
        IAmStartedByMessages<PaymentSucceededEvent>
    {
        private static readonly ILog Log = LogManager.GetLogger<ShippingSaga>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShippingSagaData> mapper)
        {
            mapper.ConfigureMapping<OrderSubmittedEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
            mapper.ConfigureMapping<PaymentSucceededEvent>(p => p.OrderId).ToSaga(s => s.OrderId);
        }

        public async Task Handle(OrderSubmittedEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle OrderSubmittedEvent");

            Data.IsOrderSubmitted = true;
            Data.OrderId = message.OrderId;
            Data.CustomerId = message.CustomerId;

            var projection = message.Products.Select(p => new ShippingSagaData.Product { Identifier = p });
            Data.Products = projection.ToList();

            await ProcessOrder(context);
        }

        public async Task Handle(PaymentSucceededEvent message, IMessageHandlerContext context)
        {
            Log.Info("Handle PaymentSucceededEvent");

            Data.OrderId = message.OrderId;
            Data.IsPaymentProcessed = true;
            await ProcessOrder(context);
        }

        private async Task ProcessOrder(IMessageHandlerContext context)
        {
            if (Data.IsOrderSubmitted && Data.IsPaymentProcessed)
            {
                await context.Send<ShipWithFedexCommand>(cmd =>
                {
                    cmd.CustomerId = Data.CustomerId;
                    cmd.OrderId = Data.CustomerId;
                    cmd.Products = Data.Products.Select(p => p.Identifier).ToList();
                });

                MarkAsComplete();
            }
        }
    }
}
