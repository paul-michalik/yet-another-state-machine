using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMaster;
using StateMaster.AbstractStates;
using StateMaster.Core;
using StateMaster.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMaster.Tests2 {

    [TestClass()]
    public class CompoundTransitions_Test
        : TestBase<CompoundTransitions_Test.States, CompoundTransitions_Test.Transitions> 
    {
        public enum States {
            S = 1,
            S1,
            S2,
            S3,
            S4,
            S5,
            S6,
            S7,
            T,
            J1,
            J2,
            J3
        }

        public enum Transitions {
            t1 = 1,
            t2,
            t3,
            t4,
            t5,
            t6,
            t7,
            t8,
            t9,
            t10,
            t11
        }

        enum Events {
            a = 1,
            b,
            c,
            e
        }

        Composite Create(Predicate<Event>[] p_Guards)
        {
            var tB = new Builder();
            var S = tB.Composite(null).ID(States.S);
            var S1 = tB.Simple(S).ID(States.S1);
            var S2 = tB.Simple(S).ID(States.S2);
            var S3 = tB.Simple(S).ID(States.S3);
            var S4 = tB.Simple(S).ID(States.S4);
            var S5 = tB.Simple(S).ID(States.S5);
            var S6 = tB.Simple(S).ID(States.S6);
            var S7 = tB.Simple(S).ID(States.S7);
            var T = tB.Terminal(S).ID(States.T);
            var J1 = tB.Junction(S).ID(States.J1);
            var J2 = tB.Junction(S).ID(States.J2);
            var J3 = tB.Junction(S).ID(States.J3);
            // Transitions
            S1.AddTransition(Events.a, J1).ID(Transitions.t1)
              .Guard(p_Guards[1])
              .AddTransition(Events.a, J2).ID(Transitions.t11)
              .Guard(p_Guards[11]);
            S2.AddTransition(Events.b, J1).ID(Transitions.t2)
              .Guard(p_Guards[2]);
            S3.AddTransition(Events.c, J2).ID(Transitions.t3)
              .Guard(p_Guards[3]);
            J1.AddTransition(J2).ID(Transitions.t9)
                .Guard(p_Guards[9]);
            J2.AddTransition(S4).ID(Transitions.t6).Guard(p_Guards[6])
              .AddTransition(J3).ID(Transitions.t5).Guard(p_Guards[5])
              .AddTransition(S6).ID(Transitions.t4).Guard(p_Guards[4]);
            J3.AddTransition(S5).ID(Transitions.t8).Guard(p_Guards[8])
              .AddTransition(S7).ID(Transitions.t7).Guard(p_Guards[7]);
            S.AddTransition(Events.e, T).ID(Transitions.t10);

            return S;
        }

        [TestMethod]
        [TestProperty("Module", "CompoundTransitions.Test")]
        public void Junction_Traversal_Forward_Test_Guards_1_9_6_True()
        {
            Machine = Create(new Predicate<Event>[] {
                _0 => false, // bogus
                _1 => true,
                _2 => false,
                _3 => false,
                _4 => false,
                _5 => false,
                _6 => true,
                _7 => false,
                _8 => false,
                _9 => true,
                _10 => true, // bogus
                _11 => false
            });

            var S1 = AllStates[States.S1];
            S1.IsActive = true;
            var tC = S1.GetEnabledCompoundTransition(Event.Create(Events.a));

            Assert.IsNotNull(tC);

            // transitions
            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t1,
                Transitions.t9,
                Transitions.t6,
            }, tC.Select(pT => (Transitions)pT.ID).ToList());

            // sources
            CollectionAssert.AreEqual(new States[] {
                States.S1,
            }, tC.Sources.Select(pS => (States)pS.ID).ToList());

            // targets
            CollectionAssert.AreEqual(new States[] {
                States.S4,
            }, tC.Targets.Select(pS => (States)pS.ID).ToList());

            // sources
            CollectionAssert.AreEqual(new States[] {
                States.S1,
            }, tC.Sources.Select(pS => (States)pS.ID).ToList());

            // MainSource
            Assert.AreEqual(States.S1, (States)tC.MainSource.ID);

            // MainTarget
            Assert.AreEqual(States.S4, (States)tC.MainTarget.ID);
        }

        [TestMethod]
        [TestProperty("Module", "CompoundTransitions.Test")]
        public void Junction_Traversal_Forward_Test_Guards_11_5_7_True()
        {
            Machine = Create(new Predicate<Event>[] {
                _0 => false, // bogus
                _1 => false,
                _2 => false,
                _3 => false,
                _4 => false,
                _5 => true,
                _6 => false,
                _7 => true,
                _8 => false,
                _9 => false,
                _10 => true, // bogus
                _11 => true
            });

            var S1 = AllStates[States.S1];
            S1.IsActive = true;
            var tC = S1.GetEnabledCompoundTransition(Event.Create(Events.a));

            Assert.IsNotNull(tC);

            // transitions
            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t11,
                Transitions.t5,
                Transitions.t7,
            }, tC.Select(pT => (Transitions)pT.ID).ToList());

            // sources
            CollectionAssert.AreEqual(new States[] {
                States.S1,
            }, tC.Sources.Select(pS => (States)pS.ID).ToList());

            // targets
            CollectionAssert.AreEqual(new States[] {
                States.S7,
            }, tC.Targets.Select(pS => (States)pS.ID).ToList());

            // sources
            CollectionAssert.AreEqual(new States[] {
                States.S1,
            }, tC.Sources.Select(pS => (States)pS.ID).ToList());

            // MainSource
            Assert.AreEqual(States.S1, (States)tC.MainSource.ID);

            // MainTarget
            Assert.AreEqual(States.S7, (States)tC.MainTarget.ID);
        }
    }
}
