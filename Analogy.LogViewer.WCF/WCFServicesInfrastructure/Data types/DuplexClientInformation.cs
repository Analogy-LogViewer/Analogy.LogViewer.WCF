using System;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure.Data_types
{
    [Serializable]
    public class DuplexClientInformation<T> : ClientInformation
    {
        public T Callback { get; }

        public DuplexClientInformation(ClientInformation clientSource, T callback) : base(clientSource)
        {
            Callback = callback;
        }
    }
}
