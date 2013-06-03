using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sitecore.ContentSearch;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Marketing.Search.XmlProvider
{
    public class XmlDocumentBuilder : AbstractDocumentBuilder<XDocument>
    {
        //START: part 6 (fix for ontime issue 386719)
        public override void AddItemFields()
        {
            if (this.Options.IndexAllFields)
            {
                this.Indexable.LoadAllFields();
            }
            if (this.IsParallel)
            {
                Parallel.ForEach<IIndexableDataField>(this.Indexable.Fields, this.ParallelOptions, new Action<IIndexableDataField>(this.CheckAndAddField));
            }
            else
            {
                foreach (IIndexableDataField field in this.Indexable.Fields)
                {
                    this.CheckAndAddField(field);
                }
            }
        }
        private void CheckAndAddField(IIndexableDataField field)
        {
            string name = field.Name;
            if ((((!this.IsTemplate || !this.Options.HasExcludedTemplateFields) || !this.Options.ExcludedTemplateFields.Contains(field.Name)) && ((!this.IsMedia || !this.Options.HasExcludedMediaFields) || !this.Options.ExcludedMediaFields.Contains(field.Name))) && (!this.Options.ExcludedFields.Contains(field.Id.ToString()) && !this.Options.ExcludedFields.Contains(name)))
            {
                if (this.Options.IndexAllFields)
                {
                    this.AddField(field);
                }
                else if (IndexOperationsHelper.IsTextField(field))
                {
                    this.AddField(field);
                }
                else if (this.Options.IncludedFields.Contains(field.Id.ToString()))
                {
                    this.AddField(field);
                }
            }
        }
        //START: part 6 (fix for ontime issue 386719)

        public XmlDocumentBuilder(IIndexable indexable, IProviderUpdateContext context) : base(indexable, context)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            Assert.ArgumentNotNull(context, "context");

            var item = (Item)(indexable as SitecoreIndexableItem);
            var tag = new XElement("item");
            tag.SetAttributeValue("name", item.Name);
            tag.SetAttributeValue("path", item.Paths.Path);
            tag.SetAttributeValue("id", item.ID);
            this.Document.Add(tag);
        }
        private bool ShouldAddField(IIndexableDataField field, XmlFieldConfiguration config)
        {
            if (!base.Index.Configuration.IndexAllFields)
            {
                if (config == null || (config.FieldName == null && config.FieldTypeName == null))
                {
                    return false;
                }
                if (field.Value == null)
                {
                    return false;
                }
            }
            return true;
        }
        public override void AddField(IIndexableDataField field)
        {
            var fieldConfig = base.Index.Configuration.FieldMap.GetFieldConfiguration(field) as XmlFieldConfiguration;
            if (fieldConfig == null || !ShouldAddField(field, fieldConfig))
            {
                return;
            }
            var tag = new XElement("field");
            var reader = base.Index.Configuration.FieldReaders.GetFieldReader(field);
            var value = reader.GetFieldValue(field).ToString();
            if (fieldConfig.StoreAsCdata)
            {
                tag.Add(new XCData(value));
            }
            else
            {
                tag.Value = value;
            }
            tag.SetAttributeValue("name", reader.GetIndexFieldName(field));
            tag.SetAttributeValue("id", field.Id.ToString());
            this.Document.Root.Add(tag);
        }

        public override void AddField(string fieldName, object fieldValue, bool append = false)
        {
        }

        public override void AddBoost()
        {
        }
    }
}
