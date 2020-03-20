using System;
using System.ServiceModel;
using System.Xml;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure.Non_Duplex_Clients
{
    public class NetTcpClient<T> : WCFNonDuplexClient<T>, IDisposable
    {
        private readonly int _sendRecievedMilisecondsTimeout;
        private readonly int _maxReceivedMessageSize;


        public NetTcpClient(string endpointAddress) 
        {
            _sendRecievedMilisecondsTimeout = 5000;
            _maxReceivedMessageSize = int.MaxValue;
            EndpointAddress = new EndpointAddress(endpointAddress);
            Binding = GetNetTCPBinding();
        }
        protected NetTcpBinding GetNetTCPBinding()
        {
            return new NetTcpBinding()
            {
                ReceiveTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                SendTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                MaxReceivedMessageSize = _maxReceivedMessageSize,
                Security = new NetTcpSecurity() { Mode = SecurityMode.None },
                ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue }

            };
        }
    }
}
