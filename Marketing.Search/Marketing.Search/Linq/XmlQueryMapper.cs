using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.ContentSearch.Linq.Helpers;
using Sitecore.ContentSearch.Linq.Nodes;
using Sitecore.ContentSearch.Linq.Parsing;

namespace Marketing.Search.Linq
{
    public class XmlQueryMapper : QueryMapper<XmlQuery>
    {
        public override XmlQuery MapQuery(IndexQuery query)
        {
            var nativeQuery  = this.HandleNode(query.RootNode);
            var result = string.Format("/items/item{0}", nativeQuery);
            return new XmlQuery(result);
        }
        protected virtual string HandleTake(TakeNode node)
        {
            var builder = new StringBuilder();
            var takeExpression = string.Format("[position()<={0}]", node.Count);
            var sourceExpression = HandleNode(node.SourceNode);
            if (sourceExpression != string.Empty)
            {
                builder.Append(sourceExpression);
            }
            builder.Append(takeExpression);
            return builder.ToString();
        }
        protected virtual string HandleEqual(EqualNode node)
        {
            var fieldNode = QueryHelper.GetFieldNode(node);
            var valueNode = QueryHelper.GetValueNode<string>(node);
            var result = string.Format("[field[@name='{0}'] = '{1}']", fieldNode.FieldKey, valueNode.Value);
            return result;
        }

        protected virtual string HandleNode(QueryNode node)
        {
            switch (node.NodeType)
            {
                case QueryNodeType.Take:
                    return HandleTake((TakeNode) node);
                case QueryNodeType.Equal:
                    return HandleEqual((EqualNode)node);
                case QueryNodeType.MatchAll:
                    return string.Empty;
            }
            throw new NotSupportedException(string.Format("The query node type '{0}' is not supported in this context.", node.NodeType));
        }
    }
}
