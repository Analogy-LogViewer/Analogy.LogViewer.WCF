using Analogy.Interfaces;
using Analogy.LogViewer.WCF.Managers;
using Analogy.LogViewer.WCF.WCFServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class ShareableComponent : IAnalogyShareable
    {

        public string OptionalTitle { get; set; } = "Analogy WCF share";
        private AnalogyClientSender proxy;

        public Task<bool> InitializeSender()
        {
            proxy = new AnalogyClientSender(UserSettingsManager.UserSettings.Settings.IP, UserSettingsManager.UserSettings.Settings.Port);

            proxy.Connect();
            return Task.FromResult(true);
        }

        public void SendMessage(AnalogyLogMessage message, string source)
        {
            proxy.SendMessage(message, source);
        }

        public void SendMessages(List<AnalogyLogMessage> messages, string source)
        {
            proxy.SendMessages(messages, source);
        }

        public void SendMessages(byte[] messages, string source)
        {
            proxy.SendMessages(messages, source);
        }

        public Task<bool> CleanupSender()
        {
            proxy.Disconnect();
            proxy.Dispose();
            return Task.FromResult(true);
        }

    }
}
