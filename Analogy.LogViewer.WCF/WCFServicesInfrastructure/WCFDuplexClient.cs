using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class WCFDuplexClient<T> : WCFBaseClient<T>, IDisposable
    {
        /// <summary>
        /// the connection  Duplex-Channel
        /// </summary>
        protected CustomDuplexClientChannel<T> DuplexChannel { get; set; }

        private Func<object, bool> _createDuplexChannelHandler;

        public WCFDuplexClient() 
        {

        }

        public WCFDuplexClient(string endpointAddress, Binding binding) : base(endpointAddress, binding)
        {

        }
        /// <summary>
        /// for non duplex client and configuration file
        /// </summary>
        /// <param name="clientEndpointConfigurationName"></param>
        /// <param name="clientConfigFile"></param>
        /// <param name="overrideEndPointofConfig"></param>
        /// <param name="logger"></param>
        public WCFDuplexClient(string clientEndpointConfigurationName, string clientConfigFile,
            string overrideEndPointofConfig) : base(clientEndpointConfigurationName, clientConfigFile,
            overrideEndPointofConfig)
        {
            InitDuplexFromConfigurationFile(this);

        }


        private void InitDuplexFromConfigurationFile(object callbacks)
        {
            try
            {
                DuplexChannel =
                    new CustomDuplexClientChannel<T>(EndpointConfigurationName, ClientConfigFile, EndpointAddress);
                RegisterToEvents();

                if (CreateDuplexChannel(callbacks))
                {
                    Logger.LogEvent(LogEnum, $"(init): SOA services duplex Client Initiated (BaseClient.ctor())");
                }
            }
            catch (Exception ex)
            {
                string error = $"Error init: SOA services duplex Client failed to send init completed:{ex.Message}";
                Logger.LogException(ex, LogEnum, error);
                throw;
            }

            _createDuplexChannelHandler = CreateDuplexChannel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CreateDuplexChannel(object callback)
        {
            const int numberOfRetry = 120;
            int currentTry = 1;
            ClientProxy = default(T);
            const int intervalWaitMiliseconds = 500;
            Logger.LogEvent(LogEnum, $"(init): Creating Channel for client: {ClientInformation}. Proxy: {ClientProxy}");
            while (currentTry < numberOfRetry)
            {
                try
                {
                    if (callback != null)
                    {
                        ClientProxy = DuplexChannel.CreateDuplexClient(callback);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, LogEnum,
                        $"(init): (Try: currentTry, Thread id: {Thread.CurrentThread.ManagedThreadId}): failed to open channel. Error: {ex.Message}.");
                    currentTry += 1;
                    Thread.Sleep(intervalWaitMiliseconds);
                    if (currentTry >= numberOfRetry)
                        throw;
                }
            }

            return false;
        }

        /// <summary>
        /// attempt to recreate Channel due to some error and log the error
        /// </summary>
        /// <param name="ex"></param>
        protected override void RecreateChannel(Exception ex)
        {
            Logger.LogException(ex, LogEnum, $"{ex} (Error): (CommunicationException). Retrying With Recreation of WCF Duplex Channel for {ClientInformation}");
            _createDuplexChannelHandler?.Invoke(this);
        }

        /// <summary>
        /// create a virtual proxy and return implementation of the service class. 
        /// provide the instance context for the callback function
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public bool ConnectWithBinding(object callBack)
        {
            _createDuplexChannelHandler = (callback) => ConnectWithBinding(callBack);
            try
            {
                DuplexChannel = new CustomDuplexClientChannel<T>(Binding, EndpointAddress);
                RegisterToEvents();
                ClientProxy = DuplexChannel.CreateDuplexClient(callBack);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogException(e, LogEnum, $"error creating Channel: {e.Message}");
                return false;
            }

        }
        #region Events logging

        private void RegisterToEvents()
        {
            if (DuplexChannel != null)
            {
                DuplexChannel.Opening += channelFactory_Opening;
                DuplexChannel.Opened += channelFactory_Opened;
                DuplexChannel.Faulted += channelFactory_Faulted;
                DuplexChannel.Closed += channelFactory_Closed;
                DuplexChannel.Closing += channelFactory_Closing;
            }
        }

        private void channelFactory_Closing(object sender, EventArgs e)
        {
            Logger.LogEvent(LogEnum, $"channel is being closed");
        }

        private void channelFactory_Closed(object sender, EventArgs e)
        {
            Logger.LogEvent(LogEnum, $"channel is closed");
        }

        private void channelFactory_Faulted(object sender, EventArgs e)
        {
            Logger.LogWarning(LogEnum, $"channel is Faulted");
        }

        private void channelFactory_Opening(object sender, EventArgs e)
        {
            Logger.LogEvent(LogEnum, $"channel is being opened");
        }

        private void channelFactory_Opened(object sender, EventArgs e)
        {
            Logger.LogEvent(LogEnum, $"channel is opened");
        }

        #endregion



        #region GetChannelStatus
        public CommunicationState GetChannelStatus()
        {
            if (DuplexChannel != null) return DuplexChannel.State;
            return CommunicationState.Faulted;
        }
        #endregion


        /// <summary>
        /// close virtual proxy
        /// </summary>
        public void Disconnect() => Dispose();

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            if (disposing)
            {
                DisposeDuplexChannel();
            }

            Disposed = true;
        }

        private void DisposeDuplexChannel()
        {
            if (DuplexChannel == null) return;
            try
            {

                if (DuplexChannel.State != CommunicationState.Faulted)
                {
                    DuplexChannel.Close();
                }
                else
                {
                    DuplexChannel.Abort();
                }
            }
            catch (CommunicationException)
            {
                DuplexChannel.Abort();
            }
            catch (TimeoutException)
            {
                DuplexChannel.Abort();
            }
            catch (Exception)
            {
                DuplexChannel.Abort();
            }
            finally
            {
                DuplexChannel = null;
            }
        }
        #endregion
    }
}
