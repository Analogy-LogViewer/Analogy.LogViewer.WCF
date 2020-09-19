using Analogy.Interfaces;
using Analogy.LogViewer.WCF.Managers;
using Analogy.LogViewer.WCF.WCFServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    class WcfReceiver : IAnalogyRealTimeDataProvider
    {
        public string OptionalTitle { get; set; }
        public Guid Id { get; set; }
        public Image ConnectedLargeImage { get; set; } = null;
        public Image ConnectedSmallImage { get; set; } = null;
        public Image DisconnectedLargeImage { get; set; } = null;
        public Image DisconnectedSmallImage { get; set; } = null;

        public event EventHandler<AnalogyDataSourceDisconnectedArgs> OnDisconnected;
        public event EventHandler<AnalogyLogMessageArgs> OnMessageReady;
        public event EventHandler<AnalogyLogMessagesArgs> OnManyMessagesReady;

        public IAnalogyOfflineDataProvider FileOperationsHandler { get; } = null;
        private IAnalogyLogger Logger { get; set; }

        private AnalogyReceiverServer _receiver;
        private bool ReceivingInProgress { get; set; }
        private ServiceHost _mSvcHost;
        public bool UseCustomColors { get; set; } = false;
        public IEnumerable<(string originalHeader, string replacementHeader)> GetReplacementHeaders()
            => Array.Empty<(string, string)>();

        public (Color backgroundColor, Color foregroundColor) GetColorForMessage(IAnalogyLogMessage logMessage)
            => (Color.Empty, Color.Empty);
        public WcfReceiver(Guid guid)
        {
            Id = guid;
            OptionalTitle = "Analogy WCF Receiver";
        }

        public Task InitializeDataProviderAsync(IAnalogyLogger logger)
        {
            Logger = logger;
            LogManager.Instance.SetLogger(logger);
            if (!ReceivingInProgress)
            {
                string address =
                    $"http://{UserSettingsManager.UserSettings.Settings.IP}:{UserSettingsManager.UserSettings.Settings.Port}/AnalogyService";
                _receiver = new AnalogyReceiverServer(address);
                _receiver.Subscription += (s, messages) =>
                {
                    List<AnalogyLogMessage> msgs = messages.ToList();
                    if (msgs.Count == 1)
                    {
                        var m = msgs.First();
                        OnMessageReady?.Invoke(this, new AnalogyLogMessageArgs(m, "", OptionalTitle, Id));
                    }
                    else
                    {
                        OnManyMessagesReady?.Invoke(this, new AnalogyLogMessagesArgs(msgs.ToList(), "", OptionalTitle));
                    }
                };
            }




            return Task.CompletedTask;
        }

        public async Task<bool> CanStartReceiving() => await Task.FromResult(true);

        public void MessageOpened(AnalogyLogMessage message)
        {
            //nop
        }

        public Task StartReceiving()
        {
            StartStopStopHost(_receiver);
            return Task.CompletedTask;
        }

        public Task StopReceiving()
        {
            OnDisconnected?.Invoke(this,
                new AnalogyDataSourceDisconnectedArgs("user disconnected", Environment.MachineName, Id));
            StartStopStopHost(_receiver);
            return Task.CompletedTask;
        }

        private void StartStopStopHost(object singletonInstance, params Uri[] baseAddresses)
        {
            if (ReceivingInProgress)
            {
                OnMessageReady?.Invoke(this,
                    new AnalogyLogMessageArgs(
                        new AnalogyLogMessage("Stop Receiving Messages", AnalogyLogLevel.Analogy,
                            AnalogyLogClass.General, "", ""), Environment.MachineName, "", Id));
                ReceivingInProgress = false;

                try
                {
                    _receiver?.CloseServiceHost();
                }
                catch (Exception ex)
                {
                    _receiver?.Dispose();
                    MessageBox.Show(@"Error: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {

                try
                {
                    _receiver.OpenServiceHost(new List<Type> { typeof(IAnalogyServiceContract) }, _receiver);
                    ReceivingInProgress = true;
                    OnMessageReady?.Invoke(this,
                        new AnalogyLogMessageArgs(
                            new AnalogyLogMessage("Server is running and listening to incoming messages",
                                AnalogyLogLevel.Analogy, AnalogyLogClass.General, "", ""),
                            Environment.MachineName, "", Id));
                }
                catch (Exception ex)
                {
                    _receiver?.CloseServiceHost();
                    MessageBox.Show(@"Error: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
