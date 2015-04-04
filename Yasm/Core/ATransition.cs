using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasm.Core {

    public enum TransitionKind {
        Internal,
        External,
        Local
    }

    public abstract class ATransition 
        : Graph.IEdge<AVertex, ATransition>
        , ICompoundTransition 
    {
        internal abstract bool CanExecute(ref Event p_Event);
        
        internal abstract void Execute(ref Event p_Event);
        
        public abstract IEnumerable<int> Triggers
        {
            get;
        }

        public bool IsEnabled(int p_TriggerID)
        {
            return Triggers.Any(p => p == p_TriggerID);
        }

        #region Core.Graph.IEdge<AVertex,ATransition> Members

        public AVertex Source
        {
            get;
            internal set;
        }

        public AVertex Target
        {
            get;
            internal set;
        }

        #endregion

        #region Core.IElement Members

        public int ID
        {
            get;
            internal set;
        }

        public ElementKind Kind
        {
            get
            {
                return ElementKind.Transition;
            }
        }

        #endregion

        #region ICompoundTransition Members

        public IEnumerable<AVertex> Sources
        {
            get { yield return Source; }
        }

        public IEnumerable<AVertex> Targets
        {
            get { yield return Target; }
        }

        public AVertex MainSource
        {
            get { return Source; }
        }

        public AVertex MainTarget
        {
            get { return Target; }
        }

        #endregion

        #region IEnumerable<ATransition> Members

        public IEnumerator<ATransition> GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
