using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Sitecore.ContentSearch.Linq.Common;

namespace Marketing.Search.Linq
{
    public class XmlQuery : IDumpable
    {
        public XmlQuery(string expression)
        {
            this.Expression = expression;
        }

        public string Expression { get; private set; }
        public void Dump(TextWriter writer)
        {
            //TODO: implement
        }
    }
}
