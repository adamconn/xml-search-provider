using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Pipelines.IndexingFilters;
using Sitecore.Events;

namespace Marketing.Search.XmlProvider
{
    public class XmlIndexOperations : IIndexOperations
    {
        //START: post 4
        public XmlIndexOperations(ISearchIndex index)
        {
            _index = index;
        }
        private readonly ISearchIndex _index;
        //END: part 4
        public void Add(IIndexable indexable, IProviderUpdateContext context, ProviderIndexConfiguration indexConfiguration)
        {
            var doc = GetDocument(indexable, context);
            //START: post 4
            if (doc == null)
            {
                Event.RaiseEvent("indexing:excludedfromindex", new object[] { _index.Name, indexable.Id });
                return;
            }
            //END: post 4
            context.AddDocument(doc, null);
        }

        public void Delete(IIndexableId id, IProviderUpdateContext context)
        {
            context.Delete(id);
        }

        public void Update(IIndexable indexable, IProviderUpdateContext context, ProviderIndexConfiguration indexConfiguration)
        {
            var doc = GetDocument(indexable, context);
            //START: post 4
            if (doc == null)
            {
                Event.RaiseEvent("indexing:excludedfromindex", new object[] { _index.Name, indexable.Id });
                return;
            }
            //END: post 4
            context.UpdateDocument(doc, null, null);
        }

        protected virtual XDocument GetDocument(IIndexable indexable, IProviderUpdateContext context)
        {
            //START: post 4
            if (InboundIndexFilterPipeline.Run(new InboundIndexFilterArgs(indexable)))
            {
                return null;
            }
            //END: post 4

            //START: commented out in post 5
            //var doc = new XDocument(
            //  new XElement("item",
            //    new XAttribute("id", item.ID.ToString()),
            //    new XAttribute("name", item.Name),
            //    new XAttribute("path", item.Paths.Path)
            //  )
            //);
            //return doc;
            //END: commented out in post 5

            //START: post 5
            var builder = new XmlDocumentBuilder(indexable, context);
            builder.AddItemFields();
            return builder.Document;
            //END: post 5

            //var item = (Item)(indexable as SitecoreIndexableItem);
            //foreach (var language in item.Languages)
            //{
            //    var latestVersion = item.Database.GetItem(item.ID, language, Sitecore.Data.Version.Latest);
            //    if (fields != null)
            //    {
            //        fields.IsLatestVersion = fields.Version == ((SitecoreIndexableItem)latestVersion).Item.Version.Number;
            //    }
            //    if (indexable is SitecoreIndexableItem)
            //    {
            //        ((SitecoreIndexableItem)indexable).IndexFieldStorageValueFormatter = context.Index.Configuration.IndexFieldStorageValueFormatter;
            //    }
            //    builder.AddSpecialField("_uniqueid", indexable.UniqueId.Value, false);
            //    builder.AddSpecialField("_datasource", indexable.DataSource, false);
            //    builder.AddSpecialField("_indexname", _index.Name, false);
            //    builder.AddSpecialFields();
            //    builder.AddItemFields();
            //    builder.AddBoost();
            //}
            //return builder.Document;
        }

        public void Delete(IIndexable indexable, IProviderUpdateContext context)
        {
            throw new NotImplementedException();
        }

        public void Delete(IIndexableUniqueId indexableUniqueId, IProviderUpdateContext context)
        {
            throw new NotImplementedException();
        }
    }
}
