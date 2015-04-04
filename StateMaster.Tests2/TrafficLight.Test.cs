using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMaster;

namespace StateMaster.Tests2 {
    [TestClass]
    public class TrafficLight {

        enum Events : int {
            Dispose,
            TurnOn,
            TurnOff,
            TimerElapsed
        }

        enum States : int {
            TL,
            TL_Init,
            On,
            On_Init,
            On_History,
            Off, 
            Red, 
            Yellow, 
            Green,
            TL_Term
        }

        public class Context {

            public AbstractStates.StateMachine Controller { get; set; }

            public Context()
            {
                Log = new List<string>();
            }

            public List<String> Log { get; private set; }

            public void OnTransition(AbstractStates.State pSource, AbstractStates.State pTarget)
            {
                Log.Add(String.Format("Transition:{0}->{1};",
                   (States)pSource.ID, (States)pTarget.ID));
            }

            public void OnEnter(AbstractStates.State pState)
            {
                Log.Add(String.Format("{0}-ENTER;",
                   (States)pState.ID));
            }

            public void OnExit(AbstractStates.State pState)
            {
                Log.Add(String.Format("{0}-EXIT;",
                   (States)pState.ID));
            }

            public void OnUnhandledEvent(Event p_Event)
            {
                Log.Add(String.Format("Event {0} Unhandled", (Events)p_Event.ID));
            }
        }

        static AbstractStates.StateMachine Create(Context p_Context)
        {
            // states
            var tBuilder = new Builder();
            var TL = tBuilder.StateMachine().ID(States.TL);
            var TL_Init = tBuilder.Initial(TL).ID(States.TL_Init);
            var TL_Term = tBuilder.Terminal(TL).ID(States.TL_Term);
            var On = tBuilder.Composite(TL).ID(States.On);
            var On_Init = tBuilder.Initial(On).ID(States.On_Init);
            var On_History = tBuilder.History(On, PseudoStates.HistoryKind.Shallow).ID(States.On_History);
            var Red = tBuilder.Simple(On).ID(States.Red);
            var Yellow = tBuilder.Simple(On).ID(States.Yellow);
            var Green = tBuilder.Simple(On).ID(States.Green);
            var Off = tBuilder.Simple(TL).ID(States.Off);

            // transitions
            TL.AddTransition(Events.Dispose, TL_Term);
            TL_Init.AddTransition(Off);
            Off.AddTransition(Events.TurnOn, On_History);
            On_History.AddTransition(Red);
            Red.AddTransition(Events.TimerElapsed, Green);
            Green.AddTransition(Events.TimerElapsed, Yellow);
            Yellow.AddTransition(Events.TimerElapsed, Red);
            On_Init.AddTransition(Red);
            On.AddTransition(Events.TurnOff, Off);

            TL.Buildee.UnhandledEvent += p_Context.OnUnhandledEvent;

            foreach (var tS in TL.Buildee) {
                var tTmpS1 = tS;
                tTmpS1.Enter += () => p_Context.OnEnter(tTmpS1);
                tTmpS1.Exit += () => p_Context.OnExit(tTmpS1);
                if (tTmpS1.Transitions != null) {
                    tTmpS1.Transitions
                        .Select(_ => _.Value)
                        .ToList()
                        .ForEach(_1 => _1.Action += (_2) => p_Context.OnTransition(tTmpS1, _1.Target));
                }
            }

            return TL;
        }

        Context m_Context;
        [TestInitialize()]
        public void SetUp()
        {
            m_Context = new Context();
            m_Context.Controller = Create(m_Context);
        }

        [TestCleanup]
        public void TearDown()
        {
            m_Context = null;
        }

        IDictionary<States, AbstractStates.State> AllStates
        {
            get
            {
                return m_Context.Controller.ToDictionary(_ => (States)_.ID);
            }
        }

        void WriteLog(bool p_Clear = false)
        {
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            if (p_Clear) 
                m_Context.Log.Clear();
        }

        void WriteHistoryStatus(PseudoStates.History p_History)
        {
            Console.Write("History: Active = {0}, Configuration:", p_History.IsHistoryActive.ToString());
            if (p_History.Count() > 0) {
                foreach (var tS in p_History) {
                    Console.Write("{0} ",
                        Enum.IsDefined(typeof(States), tS.ID) ? String.Concat((States)tS.ID) : "Undefined");
                }
            }
            Console.Write(Environment.NewLine);
        }

        Int32 HistoryCount()
        {
            var tH = AllStates[States.On_History] as PseudoStates.History;
            return tH.Count();
        } 

        bool HistoryMustContain(Int32 p_Count)
        {
            var tH = AllStates[States.On_History] as PseudoStates.History;
            return tH.Count() == p_Count;
        }

        bool HistoryMustContain(States p_ExpectedHistory)
        {
            AbstractStates.State tState;
            var tH = AllStates[States.On_History] as PseudoStates.History;
            return tH.ToDictionary(_ => (States)_.ID).TryGetValue(p_ExpectedHistory, out tState);
        }

        bool HistoryMustContain(IEnumerable<States> p_ExpectedHistory)
        {
            var tH = AllStates[States.On_History] as PseudoStates.History;
            var tD = tH.ToDictionary(_ => (States)_.ID);
            bool tContains = true;
            foreach (var tS in p_ExpectedHistory) {
                AbstractStates.State tState;
                tContains &= tD.TryGetValue(tS, out tState);
            }

            return tContains;
        }

        [TestMethod]
        [TestProperty("Module", "TrafficLight")]
        public void State_Tree_Is_As_Expected()
        {
            var tAllStates = AllStates;
            CollectionAssert.AreEquivalent(new States[] {
                States.TL,
                States.TL_Init,
                States.On,
                States.On_Init,
                States.On_History,
                States.Off, 
                States.Red, 
                States.Yellow, 
                States.Green,
                States.TL_Term
            }, m_Context.Controller.Select(_ => (States)_.ID).ToArray());

            Assert.AreEqual(tAllStates[States.On], tAllStates[States.Red].Parent);
            Assert.AreEqual(tAllStates[States.On], tAllStates[States.Yellow].Parent);
            Assert.AreEqual(tAllStates[States.On], tAllStates[States.Green].Parent);
            Assert.AreEqual(tAllStates[States.On], tAllStates[States.On_History].Parent);
            Assert.AreEqual(tAllStates[States.On], tAllStates[States.On_Init].Parent);

            Assert.AreEqual(tAllStates[States.TL], tAllStates[States.Off].Parent);
            Assert.AreEqual(tAllStates[States.TL], tAllStates[States.TL_Init].Parent);
            Assert.AreEqual(tAllStates[States.TL], tAllStates[States.TL_Term].Parent);
        }

        [TestMethod]
        [TestProperty("Module", "TrafficLight")]
        public void Enter_Initial_Confguration()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.On_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);

            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Off]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "TL_Init-ENTER;",
                "TL_Init-EXIT;",
                "Transition:TL_Init->Off;",
                "Off-ENTER;"
            }, m_Context.Log);
        }

        [TestMethod]
        [TestProperty("Module", "TrafficLight")]
        public void TurnOn()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.On_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Red]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Off-EXIT;",
                "Transition:Off->On_History;",
                "On-ENTER;",
                "On_History-ENTER;",
                "On_History-EXIT;",
                "Transition:On_History->Red;",
                "Red-ENTER;"
            }, m_Context.Log);
        }

        [TestMethod]
        [TestProperty("Module", "TrafficLight")]
        public void Fire_4_TimerEvents_And_Switch_Off()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.On_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            // Off -> Red
            m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));

            WriteLog(true);
            WriteHistoryStatus(tHistory);
            Assert.AreEqual(0, HistoryCount());

            // red -> green
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Green]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Red-EXIT;",
                "Transition:Red->Green;",
                "Green-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // green -> yellow
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Yellow]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Green-EXIT;",
                "Transition:Green->Yellow;",
                "Yellow-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // yellow -> red
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Red]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Yellow-EXIT;",
                "Transition:Yellow->Red;",
                "Red-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // red -> green
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Green]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Red-EXIT;",
                "Transition:Red->Green;",
                "Green-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // now switch off
            m_Context.Controller.HandleEvent(Event.Create(Events.TurnOff));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Green));

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Off]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Green-EXIT;",
                "On-EXIT;",
                "Transition:On->Off;",
                "Off-ENTER;"
            }, m_Context.Log);

        }

        [TestMethod]
        [TestProperty("Module", "TrafficLight")]
        public void History_Behaves_Correctly()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.On_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            // Off -> Red
            m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));

            WriteLog(true);
            WriteHistoryStatus(tHistory);
            Assert.AreEqual(0, HistoryCount());

            // 1. 
            // red -> green
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Green]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Red-EXIT;",
                "Transition:Red->Green;",
                "Green-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // switch off/on sequence begin ->
            {
                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOff));

                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(true, tHistory.IsHistoryActive);
                Assert.AreEqual(1, HistoryCount());
                Assert.AreEqual(true, HistoryMustContain(States.Green));
                Assert.AreEqual(tAllStates[States.Off], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Green-EXIT;",
                    "On-EXIT;",
                    "Transition:On->Off;",
                    "Off-ENTER;"
                }, m_Context.Log); m_Context.Log.Clear();

                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));
                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(false, tHistory.IsHistoryActive);
                Assert.AreEqual(0, HistoryCount());
                Assert.AreEqual(tAllStates[States.Green], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Off-EXIT;",
                    "Transition:Off->On_History;",
                    "On-ENTER;",
                    "On_History-ENTER;",
                    "On_History-EXIT;",
                    "Green-ENTER;"
                }, m_Context.Log); m_Context.Log.Clear();
            }
            // <- switch off sequence end

            // 2.
            // green -> yellow
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Yellow]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Green-EXIT;",
                "Transition:Green->Yellow;",
                "Yellow-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // switch off/on sequence begin ->
            {
                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOff));

                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(true, tHistory.IsHistoryActive);
                Assert.AreEqual(1, HistoryCount());
                Assert.AreEqual(true, HistoryMustContain(States.Yellow));
                Assert.AreEqual(tAllStates[States.Off], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Yellow-EXIT;",
                    "On-EXIT;",
                    "Transition:On->Off;",
                    "Off-ENTER;"
                }, m_Context.Log);
                m_Context.Log.Clear();

                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));
                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(false, tHistory.IsHistoryActive);
                Assert.AreEqual(0, HistoryCount());
                Assert.AreEqual(tAllStates[States.Yellow], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Off-EXIT;",
                    "Transition:Off->On_History;",
                    "On-ENTER;",
                    "On_History-ENTER;",
                    "On_History-EXIT;",
                    "Yellow-ENTER;"
                }, m_Context.Log);
                m_Context.Log.Clear();
            }
            // <- switch off/on sequence end

            // yellow -> red
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Red]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Yellow-EXIT;",
                "Transition:Yellow->Red;",
                "Red-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // switch off/on sequence begin ->
            {
                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOff));

                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(true, tHistory.IsHistoryActive);
                Assert.AreEqual(1, HistoryCount());
                Assert.AreEqual(true, HistoryMustContain(States.Red));
                Assert.AreEqual(tAllStates[States.Off], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Red-EXIT;",
                    "On-EXIT;",
                    "Transition:On->Off;",
                    "Off-ENTER;"
                }, m_Context.Log);
                m_Context.Log.Clear();

                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));
                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(false, tHistory.IsHistoryActive);
                Assert.AreEqual(0, HistoryCount());
                Assert.AreEqual(tAllStates[States.Red], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Off-EXIT;",
                    "Transition:Off->On_History;",
                    "On-ENTER;",
                    "On_History-ENTER;",
                    "On_History-EXIT;",
                    "Red-ENTER;"
                }, m_Context.Log);
                m_Context.Log.Clear();
            }
            // <- switch off/on sequence end

            // red -> green
            m_Context.Controller.HandleEvent(Event.Create(Events.TimerElapsed));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());

            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Green]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Red-EXIT;",
                "Transition:Red->Green;",
                "Green-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            // switch off/on sequence begin ->
            {
                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOff));

                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(true, tHistory.IsHistoryActive);
                Assert.AreEqual(1, HistoryCount());
                Assert.AreEqual(true, HistoryMustContain(States.Green));
                Assert.AreEqual(tAllStates[States.Off], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Green-EXIT;",
                    "On-EXIT;",
                    "Transition:On->Off;",
                    "Off-ENTER;"
                }, m_Context.Log);
                m_Context.Log.Clear();

                m_Context.Controller.HandleEvent(Event.Create(Events.TurnOn));
                WriteLog();
                WriteHistoryStatus(tHistory);

                Assert.AreEqual(false, tHistory.IsHistoryActive);
                Assert.AreEqual(0, HistoryCount());
                Assert.AreEqual(tAllStates[States.Green], m_Context.Controller.Configuration.First());

                CollectionAssert.AreEqual(new String[] {
                    "Off-EXIT;",
                    "Transition:Off->On_History;",
                    "On-ENTER;",
                    "On_History-ENTER;",
                    "On_History-EXIT;",
                    "Green-ENTER;"
                }, m_Context.Log);
                m_Context.Log.Clear();
            }
            // <- switch off/on sequence end
        }
    }
}
