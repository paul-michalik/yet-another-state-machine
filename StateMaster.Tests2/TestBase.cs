using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Tests2 {
    abstract public class TestBase<TStates, TTransitions>
        where TStates : IConvertible
        where TTransitions : IConvertible {
        protected StateMaster.AbstractStates.Composite Machine
        {
            get;
            set;
        }

        protected IDictionary<TStates, StateMaster.AbstractStates.State> AllStates
        {
            get
            {
                return Machine
                    .Where(_ => Enum.IsDefined(typeof(TStates), _.ID))
                    .ToDictionary(_ => (TStates)(Object)_.ID);
            }
        }

        protected IDictionary<TTransitions, Core.Transition> AllTransitions
        {
            get
            {
                return AllStates
                    .Where(pS => pS.Value.Transitions != null)
                    .SelectMany(pS => pS.Value.Transitions)
                    .Select(pP => pP.Value)
                    .Where(pT => Enum.IsDefined(typeof(TTransitions), pT.ID))
                    .ToDictionary(pT => (TTransitions)(Object)pT.ID);
            }
        }
    }
}
