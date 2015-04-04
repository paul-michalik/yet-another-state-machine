using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.InternalStates {

    internal abstract class Base : AbstractStates.Pseudo {
        //public override bool IsActive
        //{
        //    get;
        //    internal set;
        //}

        public override AbstractStates.Composite Parent
        {
            get;
            internal set;
        }

        public override Core.TransitionTable Transitions
        {
            get
            {
                return null;
            }
            internal set
            {
                throw new InvalidOperationException("Cannot access transitions of internal state");
            }
        }
    }
    
    internal sealed class Terminal : Base {
        public override int ID
        {
            get
            {
                return (Int32)Core.Constants.InternalStates.Terminal;
            }
            internal set
            {
                throw new InvalidOperationException("Cannot change ID of internal state");
            }
        }
    }
}
