using Analogy.Interfaces;
using System;
using System.ServiceModel;

namespace Analogy.LogViewer.WCF.WCFServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AnalogyReceiverServer : IAnalogyServiceContract
    {
        public EventHandler<LogMessageArgs> Subscription { get; set; }
        public void SendMessageOTA(AnalogyLogMessage message, string hostname, string dataSource)
        {
            Subscription?.Invoke(this, new LogMessageArgs(message, hostname, dataSource));
        }
    }
}
