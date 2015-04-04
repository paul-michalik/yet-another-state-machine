using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.Interfaces {
    public enum TransitionKind {
        External,
        Internal,
        Local
    }

    public interface ITransition {
        State Target { get; }
        TransitionKind Kind { get; }

        bool TryFire(IEvent p_Event, State p_Source, State p_ActualSource, out ITransitor p_Transitor);
    }

}
