using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Analogy.Interfaces;

namespace Analogy.LogViewer.WCF.WCFServices
{
    [ServiceContract]
    interface IAnalogyServiceContract
    {
        [OperationContract(IsOneWay = true)]
        void SendMessageOTA(AnalogyLogMessage message, string hostname, string dataSource);
    }
}
