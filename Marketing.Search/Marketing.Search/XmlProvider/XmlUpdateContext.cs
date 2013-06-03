using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Data;

namespace Marketing.Search.XmlProvider
{
    public class XmlUpdateContext : IProviderUpdateContext
    {
        public void AddDocument(object itemToAdd, IExecutionContext executionContext)
        {
            var doc = itemToAdd as XDocument;
            _updateDocs.Add(doc);
        }

        protected virtual void RemoveXmlFromDocument(ID id, XDocument doc)
        {
            doc.Descendants("item").Where(i => i.Attribute("id").Value == id.ToString()).Remove();
        }

        public void Delete(IIndexableId id)
        {
            _deleteIds.Add(id.Value as ID);
        }

        private List<ID> _deleteIds;

        public void Commit()
        {
            var doc1 = GetOrCreateIndexFile();
            foreach (var doc2 in _updateDocs)
            {
                AddXmlToDocument(doc1, doc2);
            }
            foreach (var id in _deleteIds)
            {
                RemoveXmlFromDocument(id, doc1);
            }
            doc1.Save(_index.IndexFilePath);
            _updateDocs.Clear();
        }

        public void Optimize()
        {
            _updateDocs = _updateDocs.Where(doc => doc.Root.Attribute("id") != null).
                                        GroupBy(doc => doc.Root.Attribute("id").Value).
                                            Select(node => node.First()).ToList();
        }

        protected virtual XDocument GetOrCreateIndexFile()
        {
            XDocument doc = null;
            if (File.Exists(_index.IndexFilePath))
            {
                doc = XDocument.Load(_index.IndexFilePath);
            }
            else
            {
                var dirPath = Path.GetDirectoryName(_index.IndexFilePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                doc = new XDocument(new XElement("items"));
            }
            return doc;
        }

        protected virtual void AddXmlToDocument(XDocument doc1, XDocument doc2)
        {
            var itemIdValue = doc2.Root.Attribute("id").Value;
            var existingNode = doc1.Descendants("item").FirstOrDefault(i => i.Attribute("id").Value == itemIdValue);
            if (existingNode != null)
            {
                existingNode.ReplaceWith(doc2.Root);
            }
            else
            {
                doc1.Root.Add(doc2.Root);
            }
        }

        public void UpdateDocument(object itemToUpdate, object criteriaForUpdate, IExecutionContext executionContext)
        {
            var doc = itemToUpdate as XDocument;
            _updateDocs.Add(doc);
        }

        public XmlUpdateContext(XmlIndex index)
        {
            _index = index;
            _updateDocs = new List<XDocument>();
            _deleteIds = new List<ID>();
        }

        private List<XDocument> _updateDocs;

        public ISearchIndex Index
        {
            get { return _index; }
        }

        private readonly XmlIndex _index;

        public void Dispose()
        {
        }

        public void Delete(IIndexableUniqueId id)
        {
            throw new NotImplementedException();
        }

        public bool IsParallel { get; private set; }
        public ParallelOptions ParallelOptions { get; private set; }
        public ICommitPolicy CommitPolicy { get; private set; }
    }
}
