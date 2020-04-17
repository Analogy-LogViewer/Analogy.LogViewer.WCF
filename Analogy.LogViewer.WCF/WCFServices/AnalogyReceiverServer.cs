using Analogy.Interfaces;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using MessagePack;

namespace Analogy.LogViewer.WCF.WCFServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AnalogyReceiverServer : IAnalogyServiceContract
    {
        public EventHandler<List<AnalogyLogMessage>> Subscription { get; set; }
        public void SendMessage(AnalogyLogMessage message, string dataSource)
        {
            Subscription?.Invoke(this, new List<AnalogyLogMessage> { message });
        }

        public void SendMessages(List<AnalogyLogMessage> messages, string dataSource)
        {
            Subscription?.Invoke(this, messages);
        }

        public void SendMessagesAsByte(byte[] messages, string dataSource)
        {
            var msgs=MessagePackSerializer.Deserialize<List<AnalogyLogMessage>>(messages);
            Subscription?.Invoke(this, msgs);
        }
    }
}
