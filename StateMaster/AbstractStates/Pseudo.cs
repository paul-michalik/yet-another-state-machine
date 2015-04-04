using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.AbstractStates {
    public abstract class Pseudo : Atomic {
        protected Pseudo() : base() {}

        internal override Core.ICompoundTransition GetEnabledCompoundTransition(Event p_Event)
        {
            throw new InvalidOperationException(
                String.Format("Cannot call GetEnabledCompoundTransition on {0}", GetType().ToString()));
        }

        public override int ID
        {
            get;
            internal set;
        }

        public override Composite Parent
        {
            get;
            internal set;
        }

        public override Core.TransitionTable Transitions
        {
            get;
            internal set;
        }
    }
}
