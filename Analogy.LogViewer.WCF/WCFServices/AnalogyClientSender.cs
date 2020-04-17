using Analogy.Interfaces;
using Analogy.LogViewer.WCF.WCFServicesInfrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analogy.LogViewer.WCF.WCFServices
{
    class AnalogyClientSender : WCFNonDuplexClient<IAnalogyServiceContract>
    {

        private static string clientEndpointConfigurationSectionName = "AnalogyService_Client";
        private static string clientConfigFile = "Analogy.LogViewer.WCF.dll.config";

        public AnalogyClientSender(string serverIP, int port) : base(clientEndpointConfigurationSectionName, clientConfigFile, $"http://{serverIP}:{port}/AnalogyService")
        {
        }


        public void SendMessage(AnalogyLogMessage message, string source)
        {
            ClientProxy.SendMessage(message, source);
        }

        public void SendMessages(IEnumerable<AnalogyLogMessage> messages, string source)
        {
            ClientProxy.SendMessages(messages, source);
        }

        public void SendMessages(byte[] messages, string source)
        {
            ClientProxy.SendMessagesAsByte(messages, source);
        }
    }
}
