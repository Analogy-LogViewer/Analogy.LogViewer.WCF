using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analogy.LogViewer.WCF
{
    [Serializable]
    public class UserSettings
    {

        public string IP { get; set; }
        public int Port { get; set; }

        public UserSettings()
        {
            IP = "127.0.0.1";
            Port = 2483;
        }
    }
}
