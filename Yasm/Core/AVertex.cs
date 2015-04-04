using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasm.Core {

    public abstract class AVertex
        : Graph.INode<AVertex, ATransition>
        , Tree.INode 
    {
        protected AVertex(ElementKind p_Kind)
        {
            m_Kind = p_Kind;
        }

        IList<ATransition> m_Transitions;
        internal IList<ATransition> Transitions
        {
            get
            {
                return m_Transitions;
            }
            set
            {
                m_Transitions = value;
            }
        }

        Region m_Region;
        public Region Region
        {
            get
            {
                return m_Region;
            }
            internal set
            {
                m_Region = value;
            }
        }

        #region Graph.INode<AVertex,ATransition> Members

        public IEnumerable<ATransition> Outgoing
        {
            get { return Transitions; }
        }

        public IEnumerable<ATransition> Incoming
        {
            get { throw new NotImplementedException(); }
        }

        public AVertex Element
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IElement Members

        public int ID
        {
            get;
            internal set;
        }

        ElementKind m_Kind;
        public ElementKind Kind
        {
            get
            {
                return m_Kind;
            }
        }

        #endregion

        #region INode Members

        public abstract IEnumerable<Tree.INode> Children
        {
            get;
        }

        public Tree.INode Parent
        {
            get
            {
                return Region;
            }
        }

        #endregion
    }
}
