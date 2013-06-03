using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Maintenance;

namespace Marketing.Search.XmlProvider
{
    public class XmlSearchIndexSummary : ISearchIndexSummary
    {
        public XmlSearchIndexSummary(XmlIndex index)
        {
            this.Index = index;
        }
        public XmlIndex Index { get; private set; }
        //START: post 7
        private XDocument GetIndexDocument()
        {
            if (!File.Exists(this.Index.IndexFilePath))
            {
                return XDocument.Parse("<items/>");
            }
            var doc = XDocument.Load(this.Index.IndexFilePath);
            return doc;
        }
        public long NumberOfDocuments
        {
            get
            {
                var doc = GetIndexDocument();
                return doc.Elements("items").Elements("item").Count();
            }
        }
        public int NumberOfFields
        {
            get
            {
                var doc = GetIndexDocument();
                return doc.Elements("items").Elements("item").Elements("field").Select(e => (string)e.Attribute("name")).Distinct().Count();
            }
        }
        public DateTime LastUpdated
        {
            get
            {
                DateTime d;
                DateTime.TryParse(this.Index.PropertyStore.Get(IndexProperties.LastUpdatedKey), out d);
                return d;
            }
            set
            {
                this.Index.PropertyStore.Set(IndexProperties.LastUpdatedKey, value.ToString(CultureInfo.InvariantCulture));
            }
        }
        //END: post 7
        public bool IsOptimized { get; private set; }
        public bool HasDeletions { get; private set; }
        public bool IsHealthy { get; private set; }
        public long NumberOfTerms { get; private set; }
        public bool IsClean { get; private set; }
        public string Directory { get; private set; }
        public bool IsMissingSegment { get; private set; }
        public int NumberOfBadSegments { get; private set; }
        public bool OutOfDateIndex { get; private set; }
        public IDictionary<string, string> UserData { get; private set; }
    }
}
