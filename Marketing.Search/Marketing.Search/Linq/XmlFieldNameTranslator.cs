using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentSearch;

namespace Marketing.Search.Linq
{
    public class XmlFieldNameTranslator : AbstractFieldNameTranslator
    {
        public override string GetIndexFieldName(MemberInfo member)
        {
            var attribute = base.GetMemberAttribute(member);
            if (attribute != null)
            {
                return this.GetIndexFieldName(attribute.GetIndexFieldName(member.Name));
            }
            return this.GetIndexFieldName(member.Name);
        }

        public override Dictionary<string, List<string>> MapDocumentFieldsToType(Type type, IEnumerable<string> documentFieldNames)
        {
            var map = documentFieldNames.ToDictionary(name => name, name => this.GetTypeFieldNames(name).ToList());
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var attribute = this.GetMemberAttribute(property);
                if (attribute == null)
                {
                    continue;
                }
                var indexFieldName = this.GetIndexFieldName(attribute.GetIndexFieldName(property.Name));
                if (map.ContainsKey(indexFieldName))
                {
                    map[indexFieldName].Add(attribute.GetTypeFieldName(property.Name));
                }
            }
            return map;
        }
        public override IEnumerable<string> GetTypeFieldNames(string fieldName)
        {
            yield return fieldName;
        }
    }
}
