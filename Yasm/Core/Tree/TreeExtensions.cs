using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core.Tree.Extensions {
    public enum TraversalMode {
        PostOrder,
        PreOrder,
        BreadthFirst
    }

    public static class TreeExtensions {

        public static IEnumerable<Tree.INode> PathToParent(
            this Tree.INode p_Node, Tree.INode p_Parent = null)
        {
            if (p_Node != null) {
                var tN = p_Node;
                for (; tN != p_Parent; tN = tN.Parent) {
                    yield return tN;
                }
                if (tN != null && tN == p_Parent)
                    yield return tN;
            }
        }

        public static IEnumerable<INode> TraverseTree(this Tree.ITree p_Tree, TraversalMode p_Mode)
        {
            if (p_Tree == null)
                throw new ArgumentNullException("p_Tree");
            return TraverseTree(p_Tree.Root, p_Mode);
        }

        public static IEnumerable<INode> TraverseTree(this Tree.INode p_Node, TraversalMode p_Mode)
        {
            if (p_Node != null) {
                switch (p_Mode) {
                    case TraversalMode.PreOrder:
                        return TraversePreOrder(p_Node);
                    case TraversalMode.PostOrder:
                        return TraversePostOrder(p_Node);
                    case TraversalMode.BreadthFirst:
                        return TraverseBreadthFirst(p_Node);
                    default:
                        throw new ArgumentException("Invalid traversal mode", "p_Mode");
                }
            } else {
                return Enumerable.Empty<Tree.INode>();
            }
        }

        static IEnumerable<INode> TraversePreOrder(Tree.INode p_Node)
        {
            if (p_Node != null) {
                yield return p_Node;
                foreach (var tN in p_Node.Children.SelectMany(TraversePreOrder)) {
                    yield return tN;
                }
            }
        }

        private static IEnumerable<INode> TraversePostOrder(Tree.INode p_Node)
        {
            if (p_Node != null) {
                return p_Node.Children.SelectMany(TraversePostOrder);
            } else {
                return Enumerable.Empty<INode>();
            }
        }

        private static IEnumerable<INode> TraverseBreadthFirst(Tree.INode p_Node)
        {
            if (p_Node != null) {
                var tQ = new Queue<Tree.INode>();
                tQ.Enqueue(p_Node);
                while (tQ.Count > 0) {
                    var tN1 = tQ.Dequeue();
                    yield return tN1;
                    foreach (var tN2 in tN1.Children) {
                        tQ.Enqueue(tN2);
                    }
                }
            }
        }
    }
}
