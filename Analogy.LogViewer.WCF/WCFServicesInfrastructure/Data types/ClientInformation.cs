using System;
using System.Diagnostics;

namespace Analogy.LogViewer.WCF.WCFServicesInfrastructure.Data_types
{
    [Serializable]
    public class ClientInformation : IEquatable<ClientInformation>
    {
        public Guid ID { get; }
        public string HostName { get; protected set; }
        public string UserName { get; protected set; }
        public string ProcessName { get; protected set; }
        public int ProcessID { get; protected set; }
        public DateTime CreationTime { get; protected set; }

        public ClientInformation()
        {
            ID = Guid.NewGuid();
            HostName = Environment.MachineName;
            UserName = "NA";//Environment.UserName;
            ProcessName = Process.GetCurrentProcess().ProcessName;
            ProcessID = Process.GetCurrentProcess().Id;
            CreationTime = DateTime.Now;
        }

        public ClientInformation(ClientInformation clientSource) : this(clientSource.ID, clientSource.HostName,
            clientSource.UserName, clientSource.ProcessName, clientSource.ProcessID)
        {

        }

        public ClientInformation(Guid clientID, string hostname, string userName, string processName, int processID) =>
                (ID, HostName, UserName, ProcessName, ProcessID, CreationTime) =
                (clientID, hostname, "NA", processName, processID, DateTime.Now);

        public override string ToString() => $" {nameof(HostName)}: {HostName}, {nameof(UserName)}: {UserName}, {nameof(ProcessName)}: {ProcessName}, {nameof(ProcessID)}: {ProcessID}, {nameof(CreationTime)}: {CreationTime}, {nameof(ID)}: {ID}";

        public bool Equals(ClientInformation other) => other != null && ID.Equals(other.ID);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ClientInformation information && Equals(information);
        }

        public override int GetHashCode() => ID.GetHashCode();
    }


}
