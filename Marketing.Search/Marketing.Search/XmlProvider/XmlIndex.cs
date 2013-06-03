using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Marketing.Search.Linq;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Events;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.ContentSearch.Maintenance.Strategies;
using Sitecore.ContentSearch.Security;
using Sitecore.Eventing;
using Sitecore.Events;
using Sitecore.IO;

namespace Marketing.Search.XmlProvider
{
    public class XmlIndex : ISearchIndex
    {
        public virtual void Rebuild()
        {
            Event.RaiseEvent("indexing:start", new object[] { this.Name, true });
            var event2 = new IndexingStartedEvent
            {
                IndexName = this.Name,
                FullRebuild = true
            };
            EventManager.QueueEvent<IndexingStartedEvent>(event2);
            this.Reset();
            this.DoRebuild();
            Event.RaiseEvent("indexing:end", new object[] { this.Name, true });
            var event3 = new IndexingFinishedEvent
            {
                IndexName = this.Name,
                FullRebuild = true
            };
            EventManager.QueueEvent<IndexingFinishedEvent>(event3);
        }
        protected virtual void DoRebuild()
        {
            //START: part 7
            var timer = new Stopwatch();
            timer.Start();
            //END: part 7
            using (var context = this.CreateUpdateContext())
            {
                foreach (var crawler in this.Crawlers)
                {
                    crawler.RebuildFromRoot(context);
                }
                context.Optimize();
                context.Commit();
            }
            //START: part 7
            timer.Stop();
            this.PropertyStore.Set(IndexProperties.RebuildTime, timer.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));
            //END: part 7
        }

        protected virtual void Reset()
        {
            if (File.Exists(this.IndexFilePath))
            {
                File.Delete(this.IndexFilePath);
            }
        }

        public void Delete(IIndexableId indexableId)
        {
            using (var context = this.CreateUpdateContext())
            {
                foreach (var crawler in this.Crawlers)
                {
                    crawler.Delete(context, indexableId);
                }
                context.Commit();
            }
        }

        public void Update(IIndexableUniqueId indexableUniqueId)
        {
            using (var context = this.CreateUpdateContext())
            {
                foreach (var crawler in this.Crawlers)
                {
                    crawler.Update(context, indexableUniqueId);
                }
                context.Commit();
            }
        }

        public void AddStrategy(IIndexUpdateStrategy strategy)
        {
            strategy.Initialize(this);
            this.Strategies.Add(strategy);
        }

        public List<IIndexUpdateStrategy> Strategies { get; private set; }

        public virtual void Refresh(IIndexable indexableStartingPoint)
        {
            using (var context = this.CreateUpdateContext())
            {
                foreach (var crawler in this.Crawlers)
                {
                    crawler.RefreshFromRoot(context, indexableStartingPoint);
                }
                context.Optimize();
                context.Commit();
            }
        }

        public IProviderUpdateContext CreateUpdateContext()
        {
            return new XmlUpdateContext(this);
        }

        public void AddCrawler(IProviderCrawler crawler)
        {
            crawler.Initialize(this);
            this.Crawlers.Add(crawler);
        }
        public List<IProviderCrawler> Crawlers { get; private set; }
        public void Initialize()
        {
            string path = null;
            if (Path.IsPathRooted(this.FolderName))
            {
                path = this.FolderName;
            }
            else
            {
                path = FileUtil.MapPath(FileUtil.MakePath(Settings.IndexFolder, this.FolderName));
            }
            this.IndexFilePath = FileUtil.MakePath(path, "index.xml");
            //START: part 7
            this.Summary = new XmlSearchIndexSummary(this);
            //END: part 7
            //START: part 8
            this.FieldNameTranslator = new XmlFieldNameTranslator();
            //END: part 8
            //START: part 9
            var config = this.Configuration as XmlIndexConfiguration;
            if (config == null)
            {
                throw new ConfigurationErrorsException("Index has no configuration.");
            }
            if (config.IndexDocumentPropertyMapper == null)
            {
                throw new ConfigurationErrorsException("IndexDocumentPropertyMapper has not been configured.");
            }
            var mapper = config.IndexDocumentPropertyMapper as ISearchIndexInitializable;
            if (mapper != null)
            {
                mapper.Initialize(this);
            }
            //END: part 9
        }

        public string IndexFilePath { get; private set; }
        public string FolderName { get; private set; }
        public XmlIndex(string name, string folderName)
        {
            this.Name = name;
            this.FolderName = folderName;
            this.Crawlers = new List<IProviderCrawler>();
            this.Strategies = new List<IIndexUpdateStrategy>();
        }

        public void SetCommitPolicy(XmlNode configNode)
        {
            throw new NotImplementedException();
        }

        public void SetCommitPolicyExecutor(XmlNode configNode)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<IIndexableUniqueId> indexableUniqueIds)
        {
            throw new NotImplementedException();
        }

        public void Delete(IIndexableUniqueId indexableUniqueId)
        {
            throw new NotImplementedException();
        }

        public IProviderDeleteContext CreateDeleteContext()
        {
            throw new NotImplementedException();
        }

        public IProviderSearchContext CreateSearchContext(SearchSecurityOptions options = SearchSecurityOptions.EnableSecurityCheck)
        {
            //START: post 9
            return new XmlSearchContext(this, options);
            //END: post 9
        }

        public string Name { get; private set; }
        public ISearchIndexSummary Summary { get; private set; }
        public ISearchIndexSchema Schema { get; private set; }
        public IIndexPropertyStore PropertyStore { get; private set; }
        public AbstractFieldNameTranslator FieldNameTranslator { get; private set; }
        public ProviderIndexConfiguration Configuration { get; set; }
        public IIndexOperations Operations { get; private set; }
 
    }
}
