using Analogy.Interfaces;
using Analogy.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class WcfFactory : IAnalogyFactory
    {
        internal static readonly Guid Id = new Guid("10AF1A71-D774-41EC-93CC-D9B798DFC51B");
        public void RegisterNotificationCallback(INotificationReporter notificationReporter)
        {
            
        }

        public Guid FactoryId { get; set; } = Id;

        public string Title { get; set; } = "WCF Receiver";

        public IEnumerable<IAnalogyChangeLog> ChangeLog { get; set; } = new List<AnalogyChangeLog>
        {
            new AnalogyChangeLog("Initial version",AnalogChangeLogType.None, "Lior Banai",new DateTime(2020, 03, 20))
        };

        public Image LargeImage { get; set; }
        public Image SmallImage { get; set; }
        public IEnumerable<string> Contributors { get; set; } = new List<string> { "Lior Banai" };
        public string About { get; set; } = "Analogy WCF Data Source";
    }
}
