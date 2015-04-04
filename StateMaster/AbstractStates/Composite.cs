
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Extensions;

namespace StateMaster.AbstractStates {

    public abstract class Composite : State, IEnumerable<State> {

        abstract internal IList<State> Children { get; set; }
        abstract internal Core.Configuration Configuration { get; set; }

        public IEnumerable<Region> Regions
        {
            get
            {
                return Children != null ? 
                    Children.OfType<Region>() : Enumerable.Empty<Region>();
            }
        }

        public Boolean IsParallel
        {
            get
            {
                return Regions.Count() > 1;
            }
        }

        public Boolean IsSequential
        {
            get
            {
                return Regions.Count() <= 1;
            }
        }

        void ExecuteHistoryStrategy(PseudoStates.HistoryEvent p_Event, Atomic p_State)
        {
            foreach (var tHistory in this.Expand().OfType<PseudoStates.History>()) {
                tHistory.HandleHistoryEvent(p_Event, this, p_State);
            }
        }

        internal void AddToConfiguration(AbstractStates.Atomic p_State)
        {
            ExecuteHistoryStrategy(PseudoStates.HistoryEvent.StateAdded, p_State);
            if (Parent != null) {
                Parent.AddToConfiguration(p_State);
            } else {
                Configuration.Add(p_State);
            }
        }

        internal void RemoveFromConfiguration(AbstractStates.Atomic p_State)
        {
            ExecuteHistoryStrategy(PseudoStates.HistoryEvent.StateRemoved, p_State);
            if (Parent != null) {
                Parent.RemoveFromConfiguration(p_State);
            } else {
                Configuration.Remove(p_State);
            }
        }

        internal bool TryEnterInitialConfiguration()
        {
            // initial state:
            var tInitial = this.Expand().OfType<PseudoStates.Initial>().FirstOrDefault();
            if (tInitial != null) {
                var tInfo = new Core.TransitionInfo {
                    Source = this, 
                    Target = tInitial, 
                    LCA = this, 
                    Kind = TransitionKind.Local
                };
                tInitial.OnEnter(ref tInfo);
                return true;
            }

            // orthogonal regions:
            foreach (var tRegion in this.Expand().OfType<Region>()) {
                var tInfo = new Core.TransitionInfo {
                    Source = this,
                    Target = tRegion,
                    LCA = this,
                    Kind = TransitionKind.Local
                };
                tRegion.OnEnter(ref tInfo);
                return true;
            }

            return false;
        }

        internal override void OnEnter(ref Core.TransitionInfo p_Info)
        {
            // There are some pecularities associated with entering a composite state directly:
            // If a composite state is explicit target of a transition then:
            // 1. it must have an explicit initial state or
            // 2. it must consist exclusively of orthogonal regions
            // If a composite state is the LCA of the transition then:
            // 1. If transition kind is external, enter the state as usual
            // 2. otherwise, do not enter (or exit) this state, but enter its initial configuration
            if (p_Info.ShouldEnterOrExit(this)) {
                ExecuteHistoryStrategy(PseudoStates.HistoryEvent.ParentEntered, null);
                base.OnEnter(ref p_Info);
            }
            
            // Note that this CAN mean, that the state is not really entered (as it was not exited previously)
            if (p_Info.ShouldEnterInitialConfiguration(this)) {
                if (!TryEnterInitialConfiguration()) {
                    throw new InvalidOperationException(
                        String.Format("Could not enter composite state {0}", this.ID.ToString()));
                }
            }
        }

        internal override void OnExit(ref Core.TransitionInfo p_Info)
        {
            if (p_Info.ShouldEnterOrExit(this)) {
                ExecuteHistoryStrategy(PseudoStates.HistoryEvent.ParentExited, null);
                base.OnExit(ref p_Info);
            }
        }

        #region IEnumerable<State> Members

        public IEnumerator<State> GetEnumerator()
        {
            foreach (var tS in this.TraversePreOrder().Where(_ => !(_ is InternalStates.Base))) {
                yield return tS;
            }
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
