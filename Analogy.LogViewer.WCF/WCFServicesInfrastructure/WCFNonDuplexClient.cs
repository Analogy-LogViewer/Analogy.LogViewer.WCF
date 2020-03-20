﻿using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure
{
    public class WCFNonDuplexClient<T> : WCFBaseClient<T>, IDisposable
    {
        /// <summary>
        /// the connection  Channel
        /// </summary>
        protected CustomClientChannel<T> Channel { get; set; }

        protected Func<bool> CreateChannelHandler;

        public WCFNonDuplexClient() 
        {

        }
        public WCFNonDuplexClient(string endpointAddress, Binding binding) : base(endpointAddress, binding)
        {

        }
        public WCFNonDuplexClient(string clientEndpointConfigurationName, string clientConfigFile,
            string overrideEndPointofConfig) : base(clientEndpointConfigurationName, clientConfigFile,
            overrideEndPointofConfig)
        {
            InitFromConfigurationFile();

        }

        private void InitFromConfigurationFile()
        {

            try
            {
                Channel = new CustomClientChannel<T>(EndpointConfigurationName, ClientConfigFile, EndpointAddress);
                if (CreateChannel())
                {
                    Logger.LogEvent(LogEnum, $"(init): SOA services Client Initiated (BaseClient.ctor())");
                }
            }
            catch (Exception ex)
            {
                string error = $"Error init: SOA services Client failed to send init completed:{ex.Message}";
                Logger.LogException(ex, LogEnum, error);
                throw;
            }

            CreateChannelHandler = CreateChannel;
        }
        private bool CreateChannel()
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

                    ClientProxy = Channel.CreateChannel();
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, LogEnum,
                        $"(init): (Try: currentTry, Thread id: {Thread.CurrentThread.ManagedThreadId}): failed to open channel. Error: {ex.Message}.");
                    currentTry += 1;
                    if (currentTry == 60)
                    {
                        //server is not up after a long time - kill it and restart
                        //KillServer();
                        //OpenServerIfNeeded();
                    }

                    Thread.Sleep(intervalWaitMiliseconds);
                    if (currentTry >= numberOfRetry)
                        throw;
                }
            }

            return false;
        }
        protected override void RecreateChannel(Exception ex)
        {
            Logger.LogException(ex, LogEnum, $"{ex} (Error): (CommunicationException). Retrying With Recreation of WCF Channel for client {ClientInformation}");
            CreateChannelHandler?.Invoke();
        }

        protected bool ConnectWithBinding()
        {
            CreateChannelHandler = ConnectWithBinding;
            try
            {
                Channel = new CustomClientChannel<T>(Binding, EndpointAddress);
                RegisterToEvents();
                ClientProxy = Channel.CreateChannel();
                return true;
            }
            catch (Exception e)
            {
                Logger.LogException(e, LogEnum, $"error creating Channel: {e.Message}");
                return false;
            }

        }

        /// <summary>
        /// close virtual proxy
        /// </summary>
        public void Disconnect() => Dispose();
        /// <summary>
        /// create the Channel. Binding need to set first
        /// </summary>
        /// <returns></returns>
        public bool Connect() => ConnectWithBinding();



        #region Events logging

        protected void RegisterToEvents()
        {
            if (Channel != null)
            {
                Channel.Opening += channelFactory_Opening;
                Channel.Opened += channelFactory_Opened;
                Channel.Faulted += channelFactory_Faulted;
                Channel.Closed += channelFactory_Closed;
                Channel.Closing += channelFactory_Closing;
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
            if (Channel != null) return Channel.State;
            return CommunicationState.Faulted;
        }
        #endregion



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
                DisposeSingleChannel();
            }

            Disposed = true;
        }

        private void DisposeSingleChannel()
        {
            if (Channel == null) return;
            try
            {


                if (Channel.State != CommunicationState.Faulted && Channel.State != CommunicationState.Closed)
                {
                    Channel.Close();
                }
                else
                {
                    Channel.Abort();
                }
            }
            catch (CommunicationException)
            {
                Channel.Abort();
            }
            catch (TimeoutException)
            {
                Channel.Abort();
            }
            catch (Exception)
            {
                Channel.Abort();
            }
            finally
            {
                Channel = null;
            }

        }
        #endregion

    }
}
