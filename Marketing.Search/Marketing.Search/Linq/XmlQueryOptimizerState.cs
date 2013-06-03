using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch.Linq.Parsing;

namespace Marketing.Search.Linq
{
    public class XmlQueryOptimizerState : QueryOptimizerState
    {
        public float Boost { get; set; }
    }
}
