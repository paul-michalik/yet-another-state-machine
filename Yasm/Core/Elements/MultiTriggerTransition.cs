using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core {
    internal sealed class MultiTriggerTransition : Detail.TransitionBase {
        internal MultiTriggerTransition() { }

        internal Int32[] Trigger { get; set; }

        sealed public override IEnumerable<int> Triggers
        {
            get { return Trigger; }
        }
    }
}
