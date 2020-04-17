using System;
using Analogy.Interfaces;
using Analogy.LogViewer.WCF.WCFServicesInfrastructure;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;

namespace Analogy.LogViewer.WCF.WCFServices
{
    class AnalogyClientSender : WCFNonDuplexClient<IAnalogyServiceContract>
    {
        private readonly int _sendRecievedMilisecondsTimeout=5000;
        private readonly int _maxReceivedMessageSize=int.MaxValue;
        private static string clientEndpointConfigurationSectionName = "AnalogyService_Client";
        private static string clientConfigFile = "Analogy.LogViewer.WCF.dll.config";

        public AnalogyClientSender(string serverIP, int port) : base(clientEndpointConfigurationSectionName, clientConfigFile, $"http://{serverIP}:{port}/AnalogyService")
        {
            Binding =    new WSDualHttpBinding()
            {
                ReceiveTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                SendTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                MaxReceivedMessageSize = _maxReceivedMessageSize,
                Security = new WSDualHttpSecurity() { Mode = WSDualHttpSecurityMode.None },
                ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue }
            };
        }


        public void SendMessage(AnalogyLogMessage message, string source)
        {
            ClientProxy.SendMessage(message, source);
        }

        public void SendMessages(List<AnalogyLogMessage> messages, string source)
        {
            ClientProxy.SendMessages(messages, source);
        }

        public void SendMessages(byte[] messages, string source)
        {
            ClientProxy.SendMessagesAsByte(messages, source);
        }
    }
}
