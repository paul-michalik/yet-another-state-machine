using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core {

    public static class Constants {
        internal enum InternalEvents : int {
            Completion = Int32.MinValue,
            Termination
        }

        internal enum InternalStates : int {
            Terminal = Int32.MinValue
        }

        static internal Event Create(Constants.InternalEvents p_Kind)
        {
            switch (p_Kind) {
                case Constants.InternalEvents.Completion:
                    return Event.Create((Int32)Constants.InternalEvents.Completion);
                case Constants.InternalEvents.Termination:
                    return Event.Create((Int32)Constants.InternalEvents.Termination);
            }

            throw new ArgumentException("Invalid internal event kind", "p_Kind");
        }
    }
}
