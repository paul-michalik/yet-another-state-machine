using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMaster {

    public sealed class StateBuilder<TState>
            where TState : AbstractStates.State 
    {
        public static implicit operator TState(StateBuilder<TState> p_State)
        {
            return p_State.Buildee;
        }

        internal TState Buildee { get; set; }
        internal Builder Builder { get; set; }

        static void AddToParent(AbstractStates.Composite p_Parent, TState p_Child)
        {
            if (p_Parent != null) {
                if (p_Parent.Children == null) {
                    p_Parent.Children = new List<AbstractStates.State>();
                }
                p_Child.Parent = p_Parent;
                p_Parent.Children.Add(p_Child);
            }
        }

        private StateBuilder() {}

        internal StateBuilder(Builder p_Builder, AbstractStates.Composite p_Parent, TState p_State)
        {
            Builder = p_Builder;
            Buildee = p_State;
            AddToParent(p_Parent, p_State);
        }

        public StateBuilder<TState> Enter(Action p_Action)
        {
            Buildee.Enter += p_Action;
            return this;
        }

        public StateBuilder<TState> Exit(Action p_Action)
        {
            Buildee.Exit += p_Action;
            return this;
        }

        public StateBuilder<TState> ID<TID>(TID p_ID)
            where TID : IConvertible
        {
            Builder.SetIDChecked(this, p_ID);
            return this;
        }

        // Add (completion) self transition
        public TransitionBuilder<TState> AddTransition()
        {
            return new TransitionBuilder<TState>(
                this,
                (Int32)Core.Constants.InternalEvents.Completion,
                Builder.CreateTransition(Buildee, Buildee));
        }

        // Add triggered self transition
        public TransitionBuilder<TState> AddTransition<TTriggerID>(TTriggerID p_TriggerID)
            where TTriggerID : IConvertible
        {
            Int32 tID = checked(Convert.ToInt32(p_TriggerID));
            return new TransitionBuilder<TState>(
                this, 
                tID, 
                Builder.CreateTransition(Buildee, Buildee));
        }

        // Add completion transition to target
        public TransitionBuilder<TState> AddTransition<UState>(
            StateBuilder<UState> p_Target)
            where UState : AbstractStates.State
        {
            return new TransitionBuilder<TState>(
                this,
                (Int32)Core.Constants.InternalEvents.Completion,
                Builder.CreateTransition(Buildee, p_Target));
        }

        // Add triggered transition to target
        public TransitionBuilder<TState> AddTransition<TTriggerID, UState>(
            TTriggerID p_TriggerID, 
            StateBuilder<UState> p_Target)
            where TTriggerID : IConvertible
            where UState : AbstractStates.State
        {
            Int32 tID = checked(Convert.ToInt32(p_TriggerID));
            return new TransitionBuilder<TState>(
                this,
                tID,
                Builder.CreateTransition(Buildee, p_Target));
        }
    }
}
