using System.Threading.Tasks;
using System.Xml.Linq;
using Divergent.ITOps.Interfaces;
using Divergent.ITOps.Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.ITOps.Handlers
{
    public class ShipWithFedexHandler : IHandleMessages<ShipWithFedexCommand>
    {
        private readonly IProvideCustomerInfo _customerInfo;
        private readonly IProvideShippingInfo _shippingInfo;
        private static readonly ILog Log = LogManager.GetLogger<ShipWithFedexHandler>();

        public ShipWithFedexHandler(IProvideCustomerInfo customerInfo, IProvideShippingInfo shippingInfo)
        {
            _customerInfo = customerInfo;
            _shippingInfo = shippingInfo;
        }

        public async Task Handle(ShipWithFedexCommand message, IMessageHandlerContext context)
        {
            Log.Info("Handle ShipWithFedexCommand");

            var shippingInfo = await _shippingInfo.GetPackageInfo(message.Products);
            var customerInfo = await _customerInfo.GetCustomerInfo(message.CustomerId);

            var fedExRequest = CreateFedexRequest(shippingInfo, customerInfo);

            await CallFedexWebService(fedExRequest);
            Log.Info($"Order {message.OrderId} shipped with Fedex");
        }

        private Task CallFedexWebService(XDocument fedExRequest)
        {
            return Task.FromResult(0);
        }

        private XDocument CreateFedexRequest(PackageInfo packageInfo, CustomerInfo customerInfo)
        {
            var shipment =
                new XElement("FedExShipment",
                    new XElement("ShipTo",
                        new XElement("Name", customerInfo.Name),
                        new XElement("Street", customerInfo.Street),
                        new XElement("City", customerInfo.City),
                        new XElement("PostalCode", customerInfo.PostalCode),
                        new XElement("Country", customerInfo.Country)),
                    new XElement("Measurements",
                        new XElement("Volume", packageInfo.Volume),
                        new XElement("Weight", packageInfo.Weight)));

            return shipment.Document;
        }
    }
}