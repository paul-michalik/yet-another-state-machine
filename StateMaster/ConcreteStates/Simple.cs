using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.ConcreteStates {
    internal sealed class Simple : AbstractStates.Simple {

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

        //volatile Boolean m_IsActive;
        //public override bool IsActive
        //{
        //    get
        //    {
        //        return m_IsActive;
        //    }
        //    internal set
        //    {
        //        m_IsActive = value;
        //    }
        //}
    }
}
