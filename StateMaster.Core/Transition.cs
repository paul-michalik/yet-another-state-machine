using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Core.Interfaces;

namespace StateMaster.Core {

    internal class Transition : Interfaces.ITransition {

        internal Predicate<Interfaces.IEvent> Guard { get; set; }
        internal Action<Interfaces.IMessage> Action { get; set; }
        internal Transition() {}

        #region ITransition Members

        public State Target
        {
            get; internal set;
        }

        public Interfaces.TransitionKind Kind
        {
            get; internal set;
        }

        public bool TryFire(
            IEvent p_Event, 
            State p_Source, 
            State p_ActualSource,
            out Interfaces.ITransitor p_Transitor)
        {
            p_Transitor = null;
            if (Guard == null || Guard(p_Event)) {
                var tLCAResult = LCASearch.Execute(p_ActualSource, Target);
                p_Transitor = new Transitor {
                    Source = Path(p_Source, p_ActualSource)
                        .Concat(tLCAResult.PathFromSourceToLCA)
                        .Where(_1 => Kind == Interfaces.TransitionKind.Internal ? _1 != tLCAResult.LCA : true),
                    Target = tLCAResult.PathFromLCAToTarget
                        .Where(_1 => Kind == Interfaces.TransitionKind.Internal ? _1 != tLCAResult.LCA : true)
                };
                return true;
            }
            return false;
        }

        #endregion

        static IEnumerable<State> Path(State p_Source, State p_ActualSource)
        {
            if (p_Source == p_ActualSource) {
                yield break;
            } else {
                for (var tS = p_Source; tS.Parent != p_ActualSource; tS = tS.Parent) {
                    yield return tS;
                }
            }
        }
    }
}
