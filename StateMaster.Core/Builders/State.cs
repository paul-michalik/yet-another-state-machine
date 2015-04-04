using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Core.Interfaces;

namespace StateMaster.Core.Builders {

    public class State<TState> where TState : State, new() {

        public abstract class Setter<T> {
            internal TState State { get { return Builder.m_State; } }
            internal State<TState> Builder { get; set; }

            internal Setter() { }

            public abstract State<TState> Set(T pValue);
        }

        class IDSetter : Setter<Int32> {
            internal IDSetter() : base() { }
            public override State<TState> Set(int pValue)
            {
                State.ID = pValue;
                return Builder;
            }
        }

        class ParentSetter : Setter<State> {
            internal ParentSetter() : base() { }
            public override State<TState> Set(State pValue)
            {
                State.Parent = pValue;
                return Builder;
            }
        }

        class TransitionSetter : Setter<KeyValuePair<Int32, ITransition>> {
            internal TransitionSetter() : base() { }
            public override State<TState> Set(KeyValuePair<Int32, ITransition> pValue)
            {
                Builder.m_TransitionTableBuilder.AddTransition(pValue.Key, pValue.Value);
                return Builder;
            }
        }

        TState m_State = new TState();
        TransitionTable.Builder m_TransitionTableBuilder = new TransitionTable.Builder();

        public State() { }

        public Setter<Int32> ID
        {
            get
            {
                return new IDSetter {
                    Builder = this
                };
            }
        }

        public Setter<State> Parent
        {
            get
            {
                return new ParentSetter {
                    Builder = this
                };
            }
        }

        public Setter<KeyValuePair<Int32, ITransition>> Transitions
        {
            get
            {
                return new TransitionSetter {
                    Builder = this
                };
            }
        }

        public TState Get()
        {
            m_State.Transitions = m_TransitionTableBuilder.Get();
            return m_State;
        }
    }
}
