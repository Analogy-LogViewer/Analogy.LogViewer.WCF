using Analogy.Interfaces;
using Analogy.Interfaces.Factories;
using System;
using System.Collections.Generic;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class WcfFactory : IAnalogyFactory
    {
        public Guid FactoryID => new Guid("10AF1A71-D774-41EC-93CC-D9B798DFC51B");

        public string Title => "WCF Receiver";

        public IAnalogyDataProvidersFactory DataProviders { get; } = new WcfDataProviderFactory();

        public IAnalogyCustomActionsFactory Actions => null;

        public IEnumerable<IAnalogyChangeLog> ChangeLog { get; } = new List<AnalogyChangeLog>
        {
            new AnalogyChangeLog("Initial version",AnalogChangeLogType.None, "Lior Banai",new DateTime(2020, 03, 20))
        };
        public IEnumerable<string> Contributors { get; } = new List<string> { "Lior Banai" };
        public string About { get; } = "Analogy WCF Data Source";
    }
}
