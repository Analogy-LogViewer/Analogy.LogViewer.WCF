using Analogy.Interfaces;
using System.ServiceModel;

namespace Analogy.LogViewer.WCF.WCFServices
{
    [ServiceContract]
    interface IAnalogyServiceContract
    {
        [OperationContract(IsOneWay = true)]
        void SendMessageOTA(AnalogyLogMessage message, string hostname, string dataSource);
    }
}
