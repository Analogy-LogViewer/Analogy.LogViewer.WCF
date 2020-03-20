using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure
{
    public class ErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException faultException = new FaultException(error.Message);
            MessageFault messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, faultException.Action);
        }

        public bool HandleError(Exception error)
        {
            return true;
        }
    }
}
