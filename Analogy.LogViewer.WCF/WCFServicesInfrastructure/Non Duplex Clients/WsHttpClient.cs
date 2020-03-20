using System;
using System.ServiceModel;
using System.Xml;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure.Non_Duplex_Clients
{
    public class WsHttpClient<T> : WCFNonDuplexClient<T>, IDisposable
    {
        private readonly int _sendRecievedMilisecondsTimeout;
        private readonly int _maxReceivedMessageSize;
        public WsHttpClient(string endpointAddress)
        {
            _sendRecievedMilisecondsTimeout = 5000;
            _maxReceivedMessageSize = int.MaxValue;
            EndpointAddress = new EndpointAddress(endpointAddress);
            Binding = GetWSDualHttpBinding();
        }


        protected WSDualHttpBinding GetWSDualHttpBinding()
        {
            return new WSDualHttpBinding()
            {
                ReceiveTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                SendTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                MaxReceivedMessageSize = _maxReceivedMessageSize,
                Security = new WSDualHttpSecurity() { Mode = WSDualHttpSecurityMode.None },
                ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue }
            };
        }
    }
}
