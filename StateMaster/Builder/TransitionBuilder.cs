using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster {

    public sealed class TransitionBuilder<TSource>
        where TSource : AbstractStates.State 
    {
        public static implicit operator Core.Transition(TransitionBuilder<TSource> p_This)
        {
            return p_This.Buildee;
        }

        readonly StateBuilder<TSource> m_SourceBuilder;
        StateBuilder<TSource> SourceBuilder {
            get
            {
                return m_SourceBuilder;
            }
        }

        private Boolean BuildeeAdded { get; set; }
        internal Core.Transition Buildee { get; set; }
        internal Int32 TriggerID { get; set; }

        TransitionBuilder<TSource> OnNewTransition(Int32 p_TriggerID, Core.Transition p_Transition)
        {
            this.TriggerID = p_TriggerID;
            this.Buildee = p_Transition;
            AddToTransitionTable(this.SourceBuilder, this.TriggerID, this.Buildee);
            return this;
        }

        static void AddToTransitionTable(
            AbstractStates.State p_Source, 
            Int32 p_TriggerID,
            Core.Transition p_Transition)
        {
            // TODO: do transition correctness check here!

            // Assumption: p_Transition.Target != null
            if (p_Source.Transitions == null) {
                p_Source.Transitions = new Core.TransitionTable() {
                    { p_TriggerID, p_Transition }
                };
            } else {
                p_Source.Transitions.Add(p_TriggerID, p_Transition);
            }
        }

        static bool TryReplaceCurrentTriggerBy(
            AbstractStates.State p_Source,
            Int32 p_OldTriggerID,
            Int32 p_NewTriggerID,
            Core.Transition p_Transition)
        {
            // Neither add, nor Remove throw:
            if (p_OldTriggerID != p_NewTriggerID && p_Source.Transitions != null) {
                if (p_Source.Transitions.Remove(p_OldTriggerID, p_Transition)) {
                    p_Source.Transitions.Add(p_NewTriggerID, p_Transition);
                    return true;
                }
            }

            return false;
        }
 
        internal TransitionBuilder(
            StateBuilder<TSource> p_SourceBuilder, 
            Int32 p_TriggerID, 
            Core.Transition p_Transition)
        {
            m_SourceBuilder = p_SourceBuilder;
            OnNewTransition(p_TriggerID, p_Transition);
        }

        public TransitionBuilder<TSource> ID<TID>(TID p_ID)
            where TID : IConvertible
        {
            SourceBuilder.Builder.SetIDChecked(this, p_ID);
            return this;
        }

        public TransitionBuilder<TSource> Guard(Predicate<Event> p_Guard)
        {
            Buildee.Guard = p_Guard;
            return this;
        }

        public TransitionBuilder<TSource> Action(Action<IMessage> p_Action)
        {
            Buildee.Action += p_Action;
            return this;
        }

        public TransitionBuilder<TSource> Kind(TransitionKind p_Kind)
        {
            Buildee.Kind = p_Kind;
            return this;
        }

        public TransitionBuilder<TSource> Trigger<TEventID>(TEventID p_EventID)
            where TEventID : IConvertible
        {
            var tNewTriggerID = checked(Convert.ToInt32(p_EventID));
            if (TryReplaceCurrentTriggerBy(SourceBuilder, TriggerID, tNewTriggerID, Buildee)) {
                TriggerID = tNewTriggerID;
            }
            return this;
        }

        // Add (completion) self transition
        public TransitionBuilder<TSource> AddTransition()
        {
            this.OnNewTransition((Int32)Core.Constants.InternalEvents.Completion,
                SourceBuilder.Builder.CreateTransition(SourceBuilder, SourceBuilder));
            return this;
        }

        // Add triggered self transition
        public TransitionBuilder<TSource> AddTransition<TTriggerID>(TTriggerID p_TriggerID)
            where TTriggerID : IConvertible
        {
            Int32 tID = checked(Convert.ToInt32(p_TriggerID));
            this.OnNewTransition(tID, 
                SourceBuilder.Builder.CreateTransition(SourceBuilder, SourceBuilder));
            return this;
        }

        // Add completion transition to target
        public TransitionBuilder<TSource> AddTransition<TTarget>(StateBuilder<TTarget> p_Target)
            where TTarget : AbstractStates.State
        {
            this.OnNewTransition((Int32)Core.Constants.InternalEvents.Completion,
                SourceBuilder.Builder.CreateTransition(SourceBuilder, p_Target));
            return this;
        }

        // Add triggered transition to target
        public TransitionBuilder<TSource> AddTransition<TEventID, TTarget>(
            TEventID p_TriggerID,
            StateBuilder<TTarget> p_Target)
            where TEventID : IConvertible
            where TTarget : AbstractStates.State
        {
            Int32 tID = checked(Convert.ToInt32(p_TriggerID));
            this.OnNewTransition(tID,
                SourceBuilder.Builder.CreateTransition(SourceBuilder, p_Target));
            return this;
        }

        // new completion transition on central builder
        public TransitionBuilder<USource> AddTransition<USource, UTarget>(
            StateBuilder<USource> p_Source,
            StateBuilder<UTarget> p_Target)
            where USource : AbstractStates.State
            where UTarget : AbstractStates.State
        {
            return p_Source.AddTransition(p_Target);
        }

        // new triggered transition on central builder
        public TransitionBuilder<USource> AddTransition<TTriggerID, USource, UTarget>(
            TTriggerID p_TriggerID,
            StateBuilder<USource> p_Source,
            StateBuilder<UTarget> p_Target)
            where TTriggerID : IConvertible
            where USource : AbstractStates.State
            where UTarget : AbstractStates.State
        {
            return p_Source.AddTransition(p_TriggerID, p_Target);
        }

        // new triggered transition on cetral builder
        public TransitionBuilder<USource> AddTransition<TTriggerID, USource, UTarget>(
            TTriggerID p_TriggerID,
            StateBuilder<USource> p_Source,
            StateBuilder<UTarget> p_Target,
            Predicate<Event> p_Guard,
            Action<IMessage> p_Action)
            where TTriggerID : IConvertible
            where USource : AbstractStates.State
            where UTarget : AbstractStates.State
        {
            return p_Source
                .AddTransition(p_TriggerID, p_Target)
                .Guard(p_Guard)
                .Action(p_Action);
        }
    }
}
