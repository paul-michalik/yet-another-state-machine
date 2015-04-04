using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.AbstractStates {
    public abstract class Atomic : State {
        protected Atomic() : base() {}

        internal override void OnEnter(ref Core.TransitionInfo p_Info)
        {
            base.OnEnter(ref p_Info);
            Parent.AddToConfiguration(this);
        }

        internal override void OnExit(ref Core.TransitionInfo p_Info)
        {
            Parent.RemoveFromConfiguration(this);
            base.OnExit(ref p_Info);
        }

        //public abstract bool IsActive { get; internal set; }
    }

    public abstract class Simple : Atomic {
        protected Simple() : base() {}
    }
}
