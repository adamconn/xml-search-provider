using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sitecore.ContentSearch;

namespace Marketing.Search.XmlProvider
{
    public class XmlFieldConfiguration : AbstractSearchFieldConfiguration
    {
        public XmlFieldConfiguration()
        {
        }
        public XmlFieldConfiguration(string name, string fieldTypeName, IDictionary<string, string> attributes, XmlNode configNode)
        {
            this.FieldName = name;
            this.FieldTypeName = fieldTypeName;
            var pair = attributes.FirstOrDefault(p => p.Key == "storageType");
            if (pair.Key != null)
            {
                if (string.Equals(pair.Value, "CDATA", StringComparison.OrdinalIgnoreCase))
                {
                    this.StoreAsCdata = true;
                }
            }
        }
        public bool StoreAsCdata { get; private set; }
    }
}
