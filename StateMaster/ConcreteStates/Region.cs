using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.ConcreteStates {
    internal sealed class Region : AbstractStates.Region {
        internal override IList<AbstractStates.State> Children
        {
            get;
            set;
        }

        internal override Core.Configuration Configuration
        {
            get;
            set;
        }

        public override int ID
        {
            get;
            internal set;
        }

        public override AbstractStates.Composite Parent
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
