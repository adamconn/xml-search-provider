using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.Data;

namespace Marketing.Search.XmlProvider
{
    public class XmlIndexFieldStorageValueFormatter : IndexFieldStorageValueFormatter
    {
        public XmlIndexFieldStorageValueFormatter()
        {
            base.AddConverter(typeof(Guid), new IndexFieldGuidValueConverter());
            base.AddConverter(typeof(ID), new IndexFieldIDValueConverter());
            base.AddConverter(typeof(DateTime), new IndexFieldDateTimeValueConverter());
            base.AddConverter(typeof(SitecoreItemId), new IndexFieldSitecoreItemIDValueConvertor(new IndexFieldIDValueConverter()));
            base.AddConverter(typeof(SitecoreItemUniqueId), new IndexFieldSitecoreItemUniqueIDValueConvertor(new IndexFieldItemUriValueConvertor()));
            base.AddConverter(typeof(ItemUri), new IndexFieldItemUriValueConvertor());
            base.EnumerableConverter = new IndexFieldEnumerableConverter(this);
        }
        public override object FormatValueForIndexStorage(object value)
        {
            return value;
        }
    }
}
