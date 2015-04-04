using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster {

    //public sealed class SourceBuilder<Transition> where Transition : IComparable<Transition>, IEquatable<Transition> {
    public sealed class Builder {
        static readonly Int32 c_InitialID = Int32.MinValue;
        public static Int32 DefaultIDMinValue
        {
            get
            {
                return c_InitialID + 1;
            }
        }

        sealed class ManageID {
            readonly SortedSet<Int32> m_IDSet = new SortedSet<Int32> { c_InitialID };

            internal void Reset()
            {
                m_IDSet.Clear();
                m_IDSet.Add(c_InitialID);
            }

            internal Int32 Next
            {
                get
                {
                    var tNewID = checked(m_IDSet.Max + 1); 
                    m_IDSet.Add(tNewID);
                    return tNewID;
                }
            }

            internal void ReplaceCurrentBy(Int32 p_CurrentID, Int32 p_NewID)
            {
                if (p_CurrentID != p_NewID && p_NewID != c_InitialID && m_IDSet.Contains(p_NewID)) {
                    throw new ArgumentException("ID must be unique", "p_NewID");
                }

                m_IDSet.Remove(p_CurrentID);
                if (p_NewID != c_InitialID)
                    m_IDSet.Add(p_NewID);
            }
        }


        readonly ManageID m_ManageStateID = new ManageID();
        readonly ManageID m_ManageTransID = new ManageID();

        public Builder() {}

        internal void SetIDChecked<TState, TID>(StateBuilder<TState> p_State, TID p_ID)
            where TState : AbstractStates.State
            where TID : IConvertible
        {
            var tID = checked(Convert.ToInt32(p_ID));
            m_ManageStateID.ReplaceCurrentBy(p_State.Buildee.ID, tID);
            p_State.Buildee.ID = tID;
        }

        internal void SetIDChecked<TSource, TID>(
            TransitionBuilder<TSource> p_Builder, TID p_ID)
            where TSource : AbstractStates.State
            where TID : IConvertible
        {
            var tID = checked(Convert.ToInt32(p_ID));
            m_ManageTransID.ReplaceCurrentBy(p_Builder.Buildee.ID, tID);
            p_Builder.Buildee.ID = tID;
        }

        StateBuilder<InternalStates.Terminal> InternalTerminal(
            StateBuilder<AbstractStates.StateMachine> p_Parent)
        {
            return new StateBuilder<InternalStates.Terminal>(
                this, p_Parent, new InternalStates.Terminal());
        }

        public StateBuilder<AbstractStates.StateMachine> StateMachine()
        {
            var tStateMachine = new StateBuilder<AbstractStates.StateMachine>(this, null,
                new ConcreteStates.StateMachine {
                    Configuration = new Core.Configuration(),
                    ID = m_ManageStateID.Next
                });

            // create internal terminal state
            tStateMachine.AddTransition(Core.Constants.InternalEvents.Termination, 
                this.InternalTerminal(tStateMachine));
            return tStateMachine;
        }

        public StateBuilder<AbstractStates.Composite> Composite(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<AbstractStates.Composite>(this, p_Parent,
                new ConcreteStates.Composite {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<AbstractStates.Region> Region(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<AbstractStates.Region>(this, p_Parent,
                new ConcreteStates.Region {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<AbstractStates.Simple> Simple(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<AbstractStates.Simple>(this, p_Parent,
                new ConcreteStates.Simple {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<PseudoStates.Initial> Initial(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<PseudoStates.Initial>(this, p_Parent,
                new PseudoStates.Initial {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<PseudoStates.Fork> Fork(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<PseudoStates.Fork>(this, p_Parent,
                new PseudoStates.Fork {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<PseudoStates.Join> Join(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<PseudoStates.Join>(this, p_Parent,
                new PseudoStates.Join {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<PseudoStates.Junction> Junction(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<PseudoStates.Junction>(this, p_Parent,
                new PseudoStates.Junction {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<PseudoStates.Terminal> Terminal(AbstractStates.Composite p_Parent)
        {
            return new StateBuilder<PseudoStates.Terminal>(this, p_Parent,
                new PseudoStates.Terminal {
                    ID = m_ManageStateID.Next
                });
        }

        public StateBuilder<PseudoStates.History> History(
            AbstractStates.Composite p_Parent, 
            PseudoStates.HistoryKind p_Kind)
        {
            return new StateBuilder<PseudoStates.History>(this, p_Parent,
                new PseudoStates.History(p_Kind) {
                    ID = m_ManageStateID.Next
                });
        }

        //internal Core.Transition CreateTransition()
        //{
        //    return new Core.Transition {
        //        ID = m_ManageTransID.Next,
        //        Kind = TransitionKind.Local
        //    };
        //}

        internal Core.Transition CreateTransition(AbstractStates.State p_Source, AbstractStates.State p_Target)
        {
            return new Core.Transition {
                ID = m_ManageTransID.Next,
                Kind = Object.ReferenceEquals(p_Source, p_Target) || p_Target == null ? 
                    TransitionKind.Internal : TransitionKind.Local,
                Target = p_Target,
                Source = p_Source
            };
        }

        /// <summary>
        /// Add completion (anonymous) transition
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="p_Source">Source state of the transition</param>
        /// <param name="p_Target">Target state of the transition</param>
        /// <returns></returns>
        public TransitionBuilder<TSource> AddTransition<TSource, TTarget>(
            StateBuilder<TSource> p_Source,
            StateBuilder<TTarget> p_Target)
            where TSource : AbstractStates.State
            where TTarget : AbstractStates.State
        {
            return p_Source.AddTransition(p_Target);
        }

        /// <summary>
        /// Add triggered transition
        /// </summary>
        /// <typeparam name="TTriggerID">Must implement IConvertible with safe conversion to Int32</typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="p_TriggerID">Trigger ID</param>
        /// <param name="p_Source">Source state of the transition</param>
        /// <param name="p_Target">Target state of the transition</param>
        /// <returns></returns>
        public TransitionBuilder<TSource> AddTransition<TTriggerID, TSource, TTarget>(
            TTriggerID p_TriggerID,
            StateBuilder<TSource> p_Source,
            StateBuilder<TTarget> p_Target)
            where TTriggerID : IConvertible
            where TSource : AbstractStates.State
            where TTarget : AbstractStates.State
        {
            return p_Source.AddTransition(p_TriggerID, p_Target);
        }

        /// <summary>
        /// Add triggered transition with guard and action
        /// </summary>
        /// <typeparam name="TTriggerID"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="p_TriggerID"></param>
        /// <param name="p_Source"> must not be null</param>
        /// <param name="p_Target"> may be equal to p_Source </param>
        /// <param name="p_Guard"> may be null </param>
        /// <param name="p_Action"> may be null </param>
        /// <returns></returns>
        public TransitionBuilder<TSource> AddTransition<TTriggerID, TSource, TTarget>(
            TTriggerID p_TriggerID,
            StateBuilder<TSource> p_Source,
            StateBuilder<TTarget> p_Target,
            Predicate<Event> p_Guard,
            Action<IMessage> p_Action)
            where TTriggerID : IConvertible
            where TSource : AbstractStates.State
            where TTarget : AbstractStates.State
        {
            return p_Source
                .AddTransition(p_TriggerID, p_Target)
                .Guard(p_Guard)
                .Action(p_Action);
        }
    }
}
