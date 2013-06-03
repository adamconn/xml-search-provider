using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Marketing.Search.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Security;
using Sitecore.Diagnostics;
using Sitecore.ContentSearch.Utilities;

namespace Marketing.Search.XmlProvider
{
    //ADDED: post 9
    public class XmlSearchContext : IProviderSearchContext
    {
        public XmlSearchContext(XmlIndex index, SearchSecurityOptions securityOptions = SearchSecurityOptions.EnableSecurityCheck)
        {
            Assert.ArgumentNotNull(index, "index");
            this.Index = index;
            this.SecurityOptions = securityOptions;
        }

        public void Dispose()
        {
        }

        public ISearchIndex Index { get; private set; }
        public SearchSecurityOptions SecurityOptions { get; private set; }
        public IQueryable<TItem> GetQueryable<TItem>() where TItem : new()
        {
            return GetQueryable<TItem>(null);
        }

        public IQueryable<TItem> GetQueryable<TItem>(IExecutionContext executionContext) where TItem : new()
        {
            var index = new LinqToXmlIndex<TItem>(this, executionContext);
            //START: logging
            if (ContentSearchConfigurationSettings.EnableSearchDebug)
            {
                var writeable = (IHasTraceWriter)index;
                writeable.TraceWriter = new LoggingTraceWriter(SearchLog.Log);
            }
            //END: logging
            return index.GetQueryable();
        }

        public IEnumerable<SearchIndexTerm> GetTermsByFieldName(string fieldName, string prefix)
        {
            throw new NotImplementedException();
        }
    }
}
