using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {
    internal interface ICompoundTransition : IEnumerable<Transition> {
        IEnumerable<AbstractStates.State> Sources { get; }
        IEnumerable<AbstractStates.State> Targets { get; }
        AbstractStates.State MainSource { get; }
        AbstractStates.State MainTarget { get; }
        Event TriggeringEvent { get; }

        void Execute(ref Event p_Event);
    }
}
