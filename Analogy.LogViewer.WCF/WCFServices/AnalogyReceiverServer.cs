using Analogy.Interfaces;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Analogy.LogViewer.WCF.WCFServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AnalogyReceiverServer : IAnalogyServiceContract
    {
        public EventHandler<IEnumerable<AnalogyLogMessage>> Subscription { get; set; }
        public void SendMessage(AnalogyLogMessage message, string dataSource)
        {
            Subscription?.Invoke(this, new List<AnalogyLogMessage> { message });
        }

        public void SendMessages(IEnumerable<AnalogyLogMessage> messages, string dataSource)
        {
            Subscription?.Invoke(this, messages);
        }

        public void SendMessages(byte[] messages, string dataSource)
        {
            //Subscription?.Invoke(this, messages);
        }
    }
}
