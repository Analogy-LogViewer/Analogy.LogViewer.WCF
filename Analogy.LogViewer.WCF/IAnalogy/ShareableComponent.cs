using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Analogy.LogViewer.WCF.Managers;
using Analogy.LogViewer.WCF.WCFServices;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class ShareableComponent: IAnalogyShareable
    {
        private AnalogyClientSender proxy;

        public Task<bool> InitializeSender()
        {
            proxy=new AnalogyClientSender(UserSettingsManager.UserSettings.Settings.IP, UserSettingsManager.UserSettings.Settings.Port);
            
            proxy.Connect();
            return Task.FromResult(true);
        }

        public void SendMessage(AnalogyLogMessage message, string source)
        {
            proxy.SendMessage(message,source);
        }

        public void SendMessages(IEnumerable<AnalogyLogMessage> messages, string source)
        {
            proxy.SendMessages(messages.ToList(),source);
        }

        public void SendMessages(byte[] messages, string source)
        {
            proxy.SendMessages(messages,source);
        }

        public Task<bool> CleanupSender()
        {
            proxy.Disconnect();
            proxy.Dispose();
            return Task.FromResult(true);
        }
    }
}
