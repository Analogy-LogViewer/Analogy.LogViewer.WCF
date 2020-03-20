using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Analogy.LogViewer.WCF.WCFServicesInfrastructure;

namespace Analogy.LogViewer.WCF.WCFServices
{
    class AnalogyClientSender : WCFNonDuplexClient<IAnalogyServiceContract>
    {
        private static string clientEndpointConfigurationSectionName = "AnalogyService_Client";
        private static string clientConfigFile = "Analogy.LogViewer.WCF.dll.config";
        public AnalogyClientSender(string serverIP,string port) : base(clientEndpointConfigurationSectionName, clientConfigFile, $"http://{serverIP}:{port}/AnalogyService")
        {
        }

        public void SendMessage(AnalogyLogMessage message, string hostname, string dataSource)
        {
            ClientProxy.SendMessageOTA(message, hostname, dataSource);
        }
    }
}
