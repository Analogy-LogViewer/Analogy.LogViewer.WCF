using Analogy.Interfaces;
using Analogy.Interfaces.Factories;
using System;
using System.Collections.Generic;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class WcfDataProviderFactory : IAnalogyDataProvidersFactory
    {
        public string Title => "Analogy WCF Receiver";

        public IEnumerable<IAnalogyDataProvider> Items => new List<IAnalogyDataProvider>
        {
            new WcfReceiver(Title, new Guid("6642B160-F992-4120-B688-B02DE2E83256")),
        };
    }
}
