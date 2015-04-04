using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core.Detail {
    internal abstract class TransitionBase : ATransition {
        internal TransitionBase() { }

        internal event Predicate<Event> Guard;

        internal event Action<Event> Effect;

        sealed internal override bool CanExecute(ref Event p_Event)
        {
            var tTmp = System.Threading.Interlocked.CompareExchange(ref Guard, null, null);
            return tTmp == null ? true : tTmp(p_Event);
        }

        sealed internal override void Execute(ref Event p_Event)
        {
            var tTmp = System.Threading.Interlocked.CompareExchange(ref Effect, null, null);
            if (tTmp != null)
                tTmp(p_Event);
        }
    }
}
