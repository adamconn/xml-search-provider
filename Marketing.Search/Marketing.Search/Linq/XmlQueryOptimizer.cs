using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.ContentSearch.Linq.Extensions;
using Sitecore.ContentSearch.Linq.Nodes;
using Sitecore.ContentSearch.Linq.Parsing;

namespace Marketing.Search.Linq
{
    public class XmlQueryOptimizer : QueryOptimizer<XmlQueryOptimizerState>
    {
        protected virtual QueryNode VisitTake(TakeNode node, XmlQueryOptimizerState state)
        {
            var sourceNode = this.Visit(node.SourceNode, state);
            return new TakeNode(sourceNode, node.Count);
        }
        protected virtual QueryNode VisitConstant(ConstantNode node, XmlQueryOptimizerState state)
        {
            var queryableType = typeof(IQueryable);
            if (node.Type.IsAssignableTo(queryableType))
            {
                return new MatchAllNode();
            }
            return node;
        }

        protected virtual QueryNode VisitWhere(WhereNode node, XmlQueryOptimizerState state)
        {
            var predicate = this.Visit(node.PredicateNode, state);
            return predicate;
        }

        protected override QueryNode Visit(QueryNode node, XmlQueryOptimizerState state)
        {
            switch (node.NodeType)
            {
                case QueryNodeType.Where:
                    return this.VisitWhere((WhereNode) node, state);
                case QueryNodeType.Take:
                    return this.VisitTake((TakeNode)node, state);
                case QueryNodeType.Constant:
                    return this.VisitConstant((ConstantNode)node, state);
            }
            return node;
        }
    }
}
