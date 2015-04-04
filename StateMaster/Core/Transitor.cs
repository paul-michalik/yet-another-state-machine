using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StateMaster.AbstractStates;
using StateMaster.Extensions;

namespace StateMaster.Core {
    internal class Transitor {

        void Exit(
            AbstractStates.StateMachine p_Machine,
            State p_LCA,
            Atomic p_Originator)
        {

        }

        void Enter(
            AbstractStates.StateMachine p_Machine,
            State p_LCA,
            State p_Target)
        {

        }

        void Execute(
            ref Event p_Event, 
            AbstractStates.StateMachine p_Machine,
            Atomic p_Actual,
            Atomic p_Originator,
            Transition p_Transition)
        {
            var tLCA = LCASearch.FindLCA(p_Actual, p_Transition.Target);

            Exit(p_Machine, tLCA, p_Originator);
            Transition.InvokeAction(p_Transition. Action, ref p_Event, null);//p_Machine;
            Enter(p_Machine, tLCA, p_Transition.Target);
        }

        void Execute(
            ref Event p_Event, 
            AbstractStates.StateMachine p_Machine,
            Atomic p_Actual,
            Atomic p_Originator,
            IEnumerable<Transition> p_EnabledTransitionSet)
        {
            foreach (var tT in p_EnabledTransitionSet) {

            }
        }

        internal bool TryProcessEvent(
            ref Event p_Event, 
            AbstractStates.StateMachine p_Machine)
        {
            return false;
        }

        static class Tuples {
            internal struct Tuple<T1, T2> {
                internal T1 Item1 { get; set; }
                internal T2 Item2 { get; set; }
            }

            internal struct Tuple<T1, T2, T3> {
                internal T1 Item1 { get; set; }
                internal T2 Item2 { get; set; }
                internal T3 Item3 { get; set; }
            }

            internal static Tuple<T1, T2> Create<T1, T2>(T1 p_1, T2 p_2)
            {
                return new Tuple<T1, T2>() {
                    Item1 = p_1,
                    Item2 = p_2
                };
            }

            internal static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 p_1, T2 p_2, T3 p_3)
            {
                return new Tuple<T1, T2, T3>() {
                    Item1 = p_1,
                    Item2 = p_2,
                    Item3 = p_3
                };
            }
        }
    }
}
