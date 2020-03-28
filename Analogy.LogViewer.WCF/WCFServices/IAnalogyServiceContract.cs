using Analogy.Interfaces;
using System.Collections.Generic;
using System.ServiceModel;

namespace Analogy.LogViewer.WCF.WCFServices
{
    [ServiceContract]
    interface IAnalogyServiceContract
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(AnalogyLogMessage message, string dataSource);
        [OperationContract(IsOneWay = true)]
        void SendMessages(IEnumerable<AnalogyLogMessage> messages, string dataSource);
        [OperationContract(IsOneWay = true)]
        void SendMessages(byte[] messages, string dataSource);
    }
}
