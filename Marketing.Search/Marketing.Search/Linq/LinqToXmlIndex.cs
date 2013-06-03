using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Marketing.Search.XmlProvider;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Indexing;
using Sitecore.ContentSearch.Linq.Parsing;
using Sitecore.ContentSearch.Security;
using Sitecore.Diagnostics;

namespace Marketing.Search.Linq
{
    public class LinqToXmlIndex<TItem> : Index<TItem, XmlQuery> 
    {
        public override TResult Execute<TResult>(XmlQuery query)
        {
            return default(TResult);
        }

        public override IEnumerable<TElement> FindElements<TElement>(XmlQuery query)
        {
            SearchLog.Log.Debug("Executing query: " + query.Expression);
            var index = _context.Index as XmlIndex;
            Assert.IsNotNull(index, "context.Index is not an instance of XmlIndex");
            var doc = new XmlDocument();
            doc.Load(index.IndexFilePath);
            var nodes = doc.SelectNodes(query.Expression);
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    //START: part 10
                    yield return this._configuration.IndexDocumentPropertyMapper.MapToType<TElement>(node, null, null, SearchSecurityOptions.DisableSecurityCheck);
                    //END: part 10
                }
            }
        }

        public LinqToXmlIndex(XmlSearchContext context) : this(context, null)
        {
        }
        public LinqToXmlIndex(XmlSearchContext context, IExecutionContext executionContext)
        {
            Assert.ArgumentNotNull(context, "context");
            _context = context;
            _configuration = (XmlIndexConfiguration)context.Index.Configuration;
            _queryOptimizer = new XmlQueryOptimizer();
            _mapper = new XmlQueryMapper();
            _fieldNameTranslator = context.Index.FieldNameTranslator;
        }

        private readonly XmlQueryMapper _mapper;
        protected override QueryMapper<XmlQuery> QueryMapper
        {
            get { return _mapper; }
        }

        private readonly XmlQueryOptimizer _queryOptimizer;
        protected override IQueryOptimizer QueryOptimizer
        {
            get { return _queryOptimizer; }
        }

        private readonly FieldNameTranslator _fieldNameTranslator;
        protected override FieldNameTranslator FieldNameTranslator
        {
            get { return _fieldNameTranslator; }
        }

        private readonly XmlSearchContext _context;
        private readonly XmlIndexConfiguration _configuration;
    }
}
