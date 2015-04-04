using System;
using System.Collections.Generic;
using System.Linq;

namespace Yasm.Core {
    internal sealed class Transition : Detail.TransitionBase {
        internal Transition() {}

        internal Int32 Trigger { get; set; }

        sealed public override IEnumerable<int> Triggers
        {
            get { yield return Trigger; }
        }
    }
}
