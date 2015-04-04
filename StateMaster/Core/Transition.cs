using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMaster {
    public enum TransitionKind {
        External,
        Internal,
        Local
    }

    namespace Core {
        internal struct TransitionInfo {
            internal AbstractStates.State Source { get; set; }
            internal AbstractStates.State Target { get; set; }
            internal AbstractStates.State LCA { get; set; }
            internal TransitionKind Kind { get; set; }

            internal bool ShouldEnterOrExit(AbstractStates.Composite p_ToBeEntered)
            {
                return !(Object.ReferenceEquals(p_ToBeEntered, LCA) && Kind != TransitionKind.External);
            }

            internal bool ShouldEnterInitialConfiguration(AbstractStates.Composite p_ToBeEntered)
            {
                return Object.ReferenceEquals(Target, p_ToBeEntered);
            }
        }

        public class Transition : ICompoundTransition {
            public Int32 ID { get; internal set; }
            public AbstractStates.State Source { get; internal set; }
            public AbstractStates.State Target { get; internal set; }
            public TransitionKind Kind { get; internal set; }
            public Action<IMessage> Action { get; internal set; }
            public Predicate<Event> Guard { get; internal set; }

            internal bool CanExecute(Event p_Event)
            {
                var tTmp = Guard;
                return tTmp != null ? tTmp(p_Event) : true;
            }

            internal void Execute(
                Event p_Event,
                AbstractStates.Atomic p_Source,
                AbstractStates.State p_ActualSource,
                AbstractStates.StateMachine p_Machine)
            {

            }

            internal void Execute(
                Event p_Event,
                AbstractStates.Atomic p_Source,
                AbstractStates.State p_ActualSource,
                IPushEventQueue p_EventQueue)
            {
                // 0. for internal transitions jump to step 3
                // 1. calculate enter, exist sets
                // 2. foreach in exit set call OnExit
                // 3. perform Action if any
                // 4. for internal transitions jump to step 6
                // 5. foreach in enter set call OnEnter
                // 6. return

                // Assume that OnEnter automatically adds a simple state to
                // active configuration of its parent
                bool tIsInternal =
                    Kind != TransitionKind.External && Object.ReferenceEquals(p_ActualSource, Target);

                if (tIsInternal) {
                    Transition.ExecuteInternal(this, ref p_Event, p_Source, p_ActualSource, p_EventQueue);
                } else {
                    Transition.ExecuteOther(this, ref p_Event, p_Source, p_ActualSource, p_EventQueue);
                }
            }

            static IEnumerable<AbstractStates.State> GetPathToActualSource(
                AbstractStates.State p_Source, 
                AbstractStates.State p_ActualSource)
            {
                for (var tS = p_Source; tS != null && tS != p_ActualSource; tS = tS.Parent) {
                    yield return tS;
                }
            }

            internal static void InvokeAction(
                Action<IMessage> p_Action, ref Event p_Event, IPushEventQueue p_EventQueue)
            {
                if (p_Action != null) {
                    p_Action(new Core.Message {
                        Queue = p_EventQueue,
                        TriggeringEvent = p_Event
                    });
                }
            }

            static void ExecuteInternal(
                Transition p_This,
                ref Event p_Event,
                AbstractStates.Atomic p_Source,
                AbstractStates.State p_ActualSource,
                IPushEventQueue p_EventQueue)
            {
                Transition.InvokeAction(p_This.Action, ref p_Event, p_EventQueue);
            }

            static void ExecuteOther(
                Transition p_This,
                ref Event p_Event,
                AbstractStates.Atomic p_Source,
                AbstractStates.State p_ActualSource,
                IPushEventQueue p_EventQueue)
            {
                var tLCAResult = LCASearch.Execute(p_ActualSource, p_This.Target);
                var tExitSet = Transition
                    .GetPathToActualSource(p_Source, p_ActualSource)
                    .Concat(tLCAResult.PathFromSourceToLCA);
                var tEntrySet = tLCAResult.PathFromLCAToTarget;

                var tInfo = new Core.TransitionInfo {
                    Source = p_Source,
                    Target = p_This.Target,
                    LCA = tLCAResult.LCA,
                    Kind = p_This.Kind
                };

                foreach (var tS in tExitSet) {
                    tS.OnExit(ref tInfo);
                }

                Transition.InvokeAction(p_This.Action, ref p_Event, p_EventQueue);

                foreach (var tS in tEntrySet) {
                    tS.OnEnter(ref tInfo);
                }
            }

            #region ICompoundTransition Members

            public IEnumerable<AbstractStates.State> Sources
            {
                get { yield return Source; }
            }

            public IEnumerable<AbstractStates.State> Targets
            {
                get { yield return Target; }
            }

            public AbstractStates.State MainSource
            {
                get { return Target; }
            }

            public AbstractStates.State MainTarget
            {
                get { return Source; }
            }

            public Event TriggeringEvent
            {
                get { throw new NotImplementedException(); }
            }

            public void Execute(ref Event p_Event)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<Transition> Members

            public IEnumerator<Transition> GetEnumerator()
            {
                yield return this;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }
    }
}
