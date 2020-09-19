using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Analogy.Interfaces;
using Analogy.LogViewer.WCF.Managers;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Multiple,
         InstanceContextMode = InstanceContextMode.Single)]
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public abstract class WCFBaseServer : IDisposable
    {
        #region Fields
        private string LogEnum = nameof(WCFBaseServer);
        private IAnalogyLogger Logger => LogManager.Instance;
        private ServiceHost _serviceHost;
        private readonly string _baseAddress;
        protected readonly int ReceiveTimeout;
        protected readonly int MaxReceivedMessageSize;

        #endregion

        #region Ctor

        /// <summary>
        /// Created to set end point definition
        /// </summary>
        /// <param name="baseAddress">Host Uri</param>
        /// <param name="logger"></param>
        public WCFBaseServer(string baseAddress)
        {
         
            _baseAddress = baseAddress;
            ReceiveTimeout = int.MaxValue;
            MaxReceivedMessageSize = int.MaxValue;
        }

        /// <summary>
        /// Created to set end point definition and other service definition
        /// </summary>
        /// <param name="baseAddress">Host Uri</param>
        /// <param name="receiveTimeout">Interval time that a connection can remain inactive before it is dropped</param>
        /// <param name="maxReceivedMessageSize">Maximum size for a recived message</param>
        /// <param name="logger"></param>
        public WCFBaseServer(string baseAddress, int receiveTimeout, int maxReceivedMessageSize)
        {
            _baseAddress = baseAddress;
            ReceiveTimeout = receiveTimeout;
            MaxReceivedMessageSize = maxReceivedMessageSize;
        }
        #endregion

        /// <summary>
        /// Open Service Host
        /// </summary>
        /// <param name="contracts">List of Interface/contracts </param>
        /// <param name="service">The class that implement the interface</param>
        public void OpenServiceHost(List<Type> contracts, object service)
        {
            if (_serviceHost != null && _serviceHost.State == CommunicationState.Opened) return;
            _serviceHost = new ServiceHost(service, new Uri(_baseAddress));
            RegisterToEvents();
            Binding binding = GetChannelBinding();
            _serviceHost.Description.Behaviors.Add(new ErrorServiceBehavior());

            foreach (var contract in contracts)
            {
                _serviceHost.AddServiceEndpoint(contract, binding, string.Empty);
            }
            _serviceHost.Open();

        }

        public CommunicationState GetServiceState() => _serviceHost.State;

        protected abstract Binding GetChannelBinding();

        #region Events logging

        private void RegisterToEvents()
        {

            if (_serviceHost != null)
            {
                _serviceHost.Faulted += channelFactory_Faulted;
                _serviceHost.Opening += channelFactory_Opening;
                _serviceHost.Opened += channelFactory_Opened;
                _serviceHost.Closed += channelFactory_Closed;
                _serviceHost.Closing += channelFactory_Closing;
            }
        }

        private void channelFactory_Closing(object sender, EventArgs e)
        {
            Logger.LogInformation( $"Server channel is being closed", LogEnum);
        }

        private void channelFactory_Closed(object sender, EventArgs e)
        {
            Logger.LogInformation($"Server channel is closed", LogEnum);
        }

        private void channelFactory_Faulted(object sender, EventArgs e)
        {
            Logger.LogWarning( $"Server channel is Faulted.", LogEnum);
        }

        private void channelFactory_Opening(object sender, EventArgs e)
        {
            Logger.LogInformation( $"Server channel is being opened", LogEnum);
        }

        private void channelFactory_Opened(object sender, EventArgs e)
        {
            Logger.LogInformation( $"Server channel is opened", LogEnum);
        }

        #endregion

        /// <summary>
        /// Close Service Host
        /// </summary>
        public void CloseServiceHost() => _serviceHost?.Close();
        #region Dispose

        private bool Disposed { get; set; }

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
                _serviceHost?.Close();
            }

            Disposed = true;
        }
        #endregion
    }

    public class NetNamedPipeWCFServer : WCFBaseServer
    {
        public NetNamedPipeWCFServer(string baseAddress) : base(baseAddress)
        {
        }

        public NetNamedPipeWCFServer(string baseAddress, int receiveTimeout, int maxReceivedMessageSize) : base(baseAddress, receiveTimeout, maxReceivedMessageSize)
        {
        }

        protected override Binding GetChannelBinding()
        {
            return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                ReceiveTimeout = TimeSpan.FromMilliseconds(ReceiveTimeout),
                MaxReceivedMessageSize = MaxReceivedMessageSize,
                Security = new NetNamedPipeSecurity(),
            };
        }
    }

    public class WSDualHttpWCFServer : WCFBaseServer
    {
        private readonly int _sendRecievedMilisecondsTimeout;
        private readonly int _maxReceivedMessageSize;
        public WSDualHttpWCFServer(string baseAddress) : base(baseAddress)
        {
            _sendRecievedMilisecondsTimeout = 5000;
            _maxReceivedMessageSize = int.MaxValue;
        }

        public WSDualHttpWCFServer(string baseAddress, int receiveTimeout, int maxReceivedMessageSize) : base(baseAddress, receiveTimeout, maxReceivedMessageSize)
        {
        }

        protected override Binding GetChannelBinding()
        {
            return new WSDualHttpBinding()
            {
                ReceiveTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                SendTimeout = TimeSpan.FromMilliseconds(_sendRecievedMilisecondsTimeout),
                MaxReceivedMessageSize = _maxReceivedMessageSize,
                Security = new WSDualHttpSecurity() { Mode = WSDualHttpSecurityMode.None },
                ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue }
            };
        }
    }
    public class WCFServer : WCFBaseServer
    {
        private Binding Binding { get; }
        public WCFServer(string baseAddress, Binding binding) : base(baseAddress)
        {
            Binding = binding;
        }

        public WCFServer(string baseAddress, Binding binding, int receiveTimeout, int maxReceivedMessageSize) : base(baseAddress, receiveTimeout, maxReceivedMessageSize)
        {
            Binding = binding;
        }

        protected override Binding GetChannelBinding() => Binding;
    }
}
