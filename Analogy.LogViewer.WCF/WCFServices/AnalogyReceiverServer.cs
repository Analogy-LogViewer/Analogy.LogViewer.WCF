using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Analogy.Interfaces;

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
