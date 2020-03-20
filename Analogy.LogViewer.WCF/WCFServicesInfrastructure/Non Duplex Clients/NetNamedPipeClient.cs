using System;
using System.ServiceModel;
using System.Xml;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure.Non_Duplex_Clients
{
    public class NetNamedPipeClient<T> : WCFNonDuplexClient<T>, IDisposable
    {

        #region Fields
        private readonly int _sendTimeout;
        private readonly int _maxReceivedMessageSize;

        #endregion

        #region Ctor

        /// <summary>
        /// Created to set end point definition
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="logger"></param>
        public NetNamedPipeClient(string endpointAddress)
        {
            EndpointAddress = new EndpointAddress(endpointAddress);
            _sendTimeout = int.MaxValue;
            _maxReceivedMessageSize = int.MaxValue;
            Binding = GetNetNamedPipeChannelBinding();
        }

        #endregion

        private NetNamedPipeBinding GetNetNamedPipeChannelBinding()
        {

            return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                SendTimeout = TimeSpan.FromMilliseconds(_sendTimeout),
                ReceiveTimeout = TimeSpan.FromMilliseconds(_sendTimeout),
                MaxReceivedMessageSize = _maxReceivedMessageSize,
                Security = new NetNamedPipeSecurity(),
                ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue }
            };
        }

    }
}
