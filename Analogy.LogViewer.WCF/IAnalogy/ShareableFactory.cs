using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Analogy.Interfaces.Factories;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class ShareableFactory : IAnalogyShareableFactory
    {
        public Guid FactoryId { get; set; } = WcfFactory.Id;
        public string Title { get; set; } = "WCF Share";
        public IEnumerable<IAnalogyShareable> Shareables { get; } = new List<IAnalogyShareable> { new ShareableComponent() };
    }
}
