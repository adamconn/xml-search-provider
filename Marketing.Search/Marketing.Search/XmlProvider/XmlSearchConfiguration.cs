using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.ContentSearch;

namespace Marketing.Search.XmlProvider
{
    public class XmlSearchConfiguration : ProviderIndexSearchConfiguration
    {
        public virtual void AddIndex(ISearchIndex index)
        {
            this.Indexes[index.Name] = index;
            index.Configuration = this.DefaultIndexConfiguration;
            index.Initialize();
        }
    }
}
