using Analogy.Interfaces;
using Analogy.Interfaces.Factories;
using System;
using System.Collections.Generic;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class WcfDataProviderFactory : IAnalogyDataProvidersFactory
    {
        public Guid FactoryId { get; set; } = WcfFactory.Id;
        public string Title { get; set; } = "Analogy WCF Receiver";
        public IEnumerable<IAnalogyDataProvider> DataProviders { get; } = new List<IAnalogyDataProvider>
        {
            new WcfReceiver(new Guid("6642B160-F992-4120-B688-B02DE2E83256")),
        };


    }
}
