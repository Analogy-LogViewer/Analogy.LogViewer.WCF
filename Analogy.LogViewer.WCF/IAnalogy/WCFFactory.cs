using Analogy.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Analogy.LogViewer.WCF.IAnalogy
{
    public class WcfFactory : Template.PrimaryFactory
    {
        internal static readonly Guid Id = new Guid("10AF1A71-D774-41EC-93CC-D9B798DFC51B");

        public override Guid FactoryId { get; set; } = Id;

        public override string Title { get; set; } = "WCF Receiver";

        public override IEnumerable<IAnalogyChangeLog> ChangeLog { get; set; } = new List<AnalogyChangeLog>
        {
            new AnalogyChangeLog("Initial version",AnalogChangeLogType.None, "Lior Banai",new DateTime(2020, 03, 20))
        };

        public override Image LargeImage { get; set; }
        public override Image SmallImage { get; set; }
        public override IEnumerable<string> Contributors { get; set; } = new List<string> { "Lior Banai" };
        public override string About { get; set; } = "Analogy WCF Data Source";
    }
}
