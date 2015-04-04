using System;
using System.Collections.Generic;
using System.Linq;

using Yasm.Core.Tree.Extensions;

namespace Yasm.Core {
    public sealed class StateMachine : Tree.INode, Tree.ITree, Graph.IGraph<AVertex, ATransition> {
        internal StateMachine(Tree.INode p_Owner) 
        {
            m_Owner = p_Owner;
        }

        readonly Tree.INode m_Owner;
        Tree.INode Owner
        {
            get
            {
                return m_Owner;
            }
        }

        IList<Region> m_Regions;
        internal IList<Region> Regions
        {
            get
            {
                return m_Regions;
            }
            set
            {
                m_Regions = value;
            }
        }

        #region INode Members

        public IEnumerable<Tree.INode> Children
        {
            get 
            {
                return m_Regions ?? Enumerable.Empty<Region>();
            }
        }

        public Tree.INode Parent
        {
            get 
            {
                return m_Owner;
            }
        }

        #endregion

        #region IElement Members

        public int ID
        {
            get;
            internal set;
        }

        public ElementKind Kind
        {
            get 
            { 
                return ElementKind.StateMachine; 
            }
        }

        #endregion

        #region ITree Members

        public Tree.INode Root
        {
            get 
            { 
                return this; 
            }
        }

        #endregion

        #region IGraph<AVertex,ATransition> Members

        public IEnumerable<AVertex> Vertices
        {
            get 
            {
                foreach (var tN in this.Root.TraverseTree(TraversalMode.PreOrder)) {
                    var tV = tN as AVertex;
                    if (tV != null) {
                        yield return tV;
                    }
                }
            }
        }

        public IEnumerable<ATransition> Edges
        {
            get 
            {
                foreach (var tV in this.Vertices)
                    foreach (var tT in tV.Outgoing)
                        yield return tT;
            }
        }

        #endregion
    }
}
