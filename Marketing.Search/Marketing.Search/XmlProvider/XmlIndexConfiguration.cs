using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.ContentSearch;

namespace Marketing.Search.XmlProvider
{
    public class XmlIndexConfiguration : ProviderIndexConfiguration
    {
        public IIndexDocumentPropertyMapper<XmlNode> IndexDocumentPropertyMapper { get; set; }
    }
}
