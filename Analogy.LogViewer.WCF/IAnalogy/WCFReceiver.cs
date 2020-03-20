using System;
using System.Drawing;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Analogy.Interfaces;
using Analogy.LogViewer.WCF.Managers;
using Analogy.LogViewer.WCF.WCFServices;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    class WCFReceiver : IAnalogyRealTimeDataProvider
    {

        public string OptionalTitle { get; }
        public Guid ID { get; }
        public bool IsConnected => true;
        public event EventHandler<AnalogyDataSourceDisconnectedArgs> OnDisconnected;
        public event EventHandler<AnalogyLogMessageArgs> OnMessageReady;
        public event EventHandler<AnalogyLogMessagesArgs> OnManyMessagesReady;

        public IAnalogyOfflineDataProvider FileOperationsHandler { get; }
        private IAnalogyLogger Logger { get; set; }

        private AnalogyReceiverServer receiver;
        private bool ReceiveingInProgress;
        private ServiceHost _mSvcHost;

        public WCFReceiver(string prefix, Guid guid)
        {
            ID = guid;
            OptionalTitle = $"{prefix}";
        }

        public Task InitializeDataProviderAsync(IAnalogyLogger logger)
        {
            Logger = logger;
            LogManager.Instance.SetLogger(logger);
            if (!ReceiveingInProgress)
            {
                receiver = new AnalogyReceiverServer();
                receiver.Subscription += (s, m) =>
                {
                    m.Message.Text = $"{m.Message.Text}. Received from Analogy hostname: {m.HostName}";
                    OnMessageReady?.Invoke(this, new AnalogyLogMessageArgs(m.Message, m.HostName, m.DataSource, ID));
                };
            }

            StartStopStopHost(receiver);
            return Task.CompletedTask;
        }

        public async Task<bool> CanStartReceiving() => await Task.FromResult(true);

        public void MessageOpened(AnalogyLogMessage message)
        {
            //nop
        }

        public void StartReceiving()
        {
            InitializeDataProviderAsync(Logger);
        }

        public void StopReceiving()
        {
            OnDisconnected?.Invoke(this,
                new AnalogyDataSourceDisconnectedArgs("user disconnected", Environment.MachineName, ID));
            StartStopStopHost(receiver);
        }

        private void StartStopStopHost(object singletonInstance, params Uri[] baseAddresses)
        {
            if (ReceiveingInProgress)
            {
                OnMessageReady?.Invoke(this,
                    new AnalogyLogMessageArgs(
                        new AnalogyLogMessage("Stop Receiving Messages", AnalogyLogLevel.AnalogyInformation,
                            AnalogyLogClass.General, "", ""), Environment.MachineName, "", ID));
                ReceiveingInProgress = false;

                try
                {
                    _mSvcHost.Close();
                }
                catch (Exception ex)
                {
                    _mSvcHost?.Abort();
                    MessageBox.Show(@"Error: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {

                try
                {
                    _mSvcHost = new ServiceHost(singletonInstance, baseAddresses);
                    _mSvcHost.Open();
                    ReceiveingInProgress = true;
                    OnMessageReady?.Invoke(this,
                        new AnalogyLogMessageArgs(
                            new AnalogyLogMessage("Server is running and listening to incoming messages",
                                AnalogyLogLevel.AnalogyInformation, AnalogyLogClass.General, "", ""),
                            Environment.MachineName, "", ID));
                }
                catch (Exception ex)
                {
                    _mSvcHost?.Abort();
                    MessageBox.Show(@"Error: " + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
