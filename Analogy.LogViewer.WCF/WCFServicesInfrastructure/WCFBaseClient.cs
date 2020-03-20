using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Analogy.Interfaces;
using Analogy.LogViewer.WCF.Managers;
using Analogy.LogViewer.WCF.WCFServicesInfrastructure.Data_types;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public abstract class WCFBaseClient<T>
    {
        #region Data members
        protected EndpointAddress EndpointAddress;
        protected Binding Binding;

        protected string EndpointConfigurationName;
        protected T ClientProxy;
        protected string ClientConfigFile;
        #endregion

        #region Properties
        protected ClientInformation ClientInformation { get; } = new ClientInformation();
        protected string LogEnum { get; set; } = nameof(WCFBaseClient<T>);
        protected IAnalogyLogger Logger => LogManager.Instance;
        protected bool Disposed { get; set; }
        #endregion
        #region Ctor

        /// <summary>
        /// Created to set end point definition
        /// </summary>
        /// <param name="logger"></param>
        public WCFBaseClient()
        {
        }
        /// <summary>
        /// Created to set end point definition
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="binding"></param>
        /// <param name="logger"></param>
        public WCFBaseClient(string endpointAddress, Binding binding)
        {
            Binding = binding;
            EndpointAddress = new EndpointAddress(endpointAddress);

        }

        /// <summary>
        /// for non duplex client and configuration file
        /// </summary>
        /// <param name="clientEndpointConfigurationName"></param>
        /// <param name="clientConfigFile"></param>
        /// <param name="overrideEndPointofConfig"></param>
        /// <param name="logger"></param>
        public WCFBaseClient(string clientEndpointConfigurationName, string clientConfigFile, string overrideEndPointofConfig)
        {
       
            EndpointConfigurationName = clientEndpointConfigurationName;
            ClientConfigFile = clientConfigFile;
            if (!string.IsNullOrEmpty(overrideEndPointofConfig))
                EndpointAddress = new EndpointAddress(overrideEndPointofConfig);
        }


        #endregion

        #region Bindings type

        #endregion

        /// <summary>
        /// attempt to recreate Channel due to some error and log the error
        /// </summary>
        /// <param name="ex"></param>
        protected abstract void RecreateChannel(Exception ex);



        protected virtual void AdditionalAction()
        {
        }




        /// <summary>
        /// run command
        /// </summary>
        /// <typeparam name="TS">The function return  type</typeparam>
        /// <param name="action">The function to run</param>
        /// <returns></returns>
        public TS ExecuteCommand<TS>(Func<TS> action)
        {
            try
            {

                return action.Invoke();
            }
            catch (Exception ce)
            {
                RecreateChannel(ce);
                AdditionalAction();
                return action.Invoke();
            }
        }
    }
}
