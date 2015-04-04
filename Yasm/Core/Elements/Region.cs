using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasm.Core {
    public sealed class Region : Tree.INode {
        internal Region(Tree.INode p_Owner)
        {
            m_Owner = p_Owner;
        }

        readonly Tree.INode m_Owner;
        public Tree.INode Owner
        {
            get
            {
                return m_Owner;
            }
        }

        public StateMachine StateMachine
        {
            get
            {
                return m_Owner.Kind == ElementKind.StateMachine ?
                    (StateMachine)m_Owner : null;
            }
        }

        public State State
        {
            get
            {
                return m_Owner.Kind == ElementKind.State ?
                    (State)m_Owner : null;
            }
        }

        internal IList<AVertex> Vertices
        {
            get;
            set;
        }

        #region Tree.INode Members

        public IEnumerable<Tree.INode> Children
        {
            get { return Vertices; }
        }

        public Tree.INode Parent
        {
            get { return m_Owner; }
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
                return ElementKind.Region;
            }
        }

        #endregion
    }
}
