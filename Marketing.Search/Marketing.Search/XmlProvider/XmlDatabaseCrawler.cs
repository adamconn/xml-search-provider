using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;

namespace Marketing.Search.XmlProvider
{
    public class XmlDatabaseCrawler : AbstractProviderCrawler
    {
        public override void Initialize(ISearchIndex index)
        {
            base.Initialize(index);
            //START: post 4
            if (this.Operations == null)
            {
                this.Operations = new XmlIndexOperations(index);
            }
            //END: post 4
            var msg = string.Format("[Index={0}] Initializing XmlDatabaseCrawler. DB:{1} / Root:{2}", index.Name, base.Database, base.Root);
            CrawlingLog.Log.Info(msg, null);
        }
    }
}
