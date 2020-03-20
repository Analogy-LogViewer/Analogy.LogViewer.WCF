using System;
using System.Collections.Generic;
using Analogy.Interfaces;
using Analogy.Interfaces.Factories;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class WCFDataProviderFactory : IAnalogyDataProvidersFactory
    {
        public string Title => "Analogy WCF Receiver";

        public IEnumerable<IAnalogyDataProvider> Items => new List<IAnalogyDataProvider>
        {
            new WCFReceiver(Title, new Guid("6642B160-F992-4120-B688-B02DE2E83256")),
        };
    }
}
