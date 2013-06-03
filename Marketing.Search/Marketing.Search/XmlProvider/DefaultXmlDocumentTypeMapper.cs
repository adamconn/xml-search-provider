using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;

namespace Marketing.Search.XmlProvider
{
    public class DefaultXmlDocumentTypeMapper : DefaultDocumentMapper<XmlNode>
    {
        protected override IEnumerable<string> GetDocumentFieldNames(XmlNode document)
        {
            var names = new List<string>();
            var fieldNodes = document.SelectNodes("field");
            names.Add("name");
            names.Add("path");
            names.Add("id");
            if (fieldNodes != null)
            {
                foreach (XmlNode fieldNode in fieldNodes)
                {
                    var fieldNameAttribute = fieldNode.Attributes["name"];
                    if (fieldNameAttribute != null)
                    {
                        names.Add(fieldNameAttribute.Value);
                    }
                }
            }
            return names;
        }

        protected override void ReadDocumentFields<TElement>(XmlNode document, IEnumerable<string> fieldNames, DocumentTypeMapInfo documentTypeMapInfo, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors, TElement result)
        {
            if (fieldNames != null)
            {
                //
                //get values stored in field tags
                var mapping = base.index.FieldNameTranslator.MapDocumentFieldsToType(result.GetType(), fieldNames);
                foreach (var pair in mapping)
                {
                    var fieldNode = document.SelectSingleNode(string.Format("field[@name='{0}']", pair.Key));
                    if (fieldNode != null)
                    {
                        foreach (var name in pair.Value)
                        {
                            documentTypeMapInfo.SetProperty(result, name, pair.Key, fieldNode.InnerText);
                        }
                    }
                }
                //
                //get values stored in attributes
                var nameAttribute = document.Attributes["name"];
                if (nameAttribute != null)
                {
                    documentTypeMapInfo.SetProperty(result, "name", "name", nameAttribute.Value);
                }
                var pathAttribute = document.Attributes["path"];
                if (pathAttribute != null)
                {
                    documentTypeMapInfo.SetProperty(result, "path", "path", pathAttribute.Value);
                }
                var idAttribute = document.Attributes["id"];
                if (idAttribute != null)
                {
                    documentTypeMapInfo.SetProperty(result, "id", "id", idAttribute.Value);
                }
            }
        }
    }
}
