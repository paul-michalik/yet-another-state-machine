using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StateMaster.Tests2 {
    /// <summary>
    /// Summary description for Boost_MSM_Player
    /// </summary>
    [TestClass]
    public class Boost_MSM_Player {

        Context m_Context;

        enum States : int {
            Player = 0,
            Player_Initial,
            Empty,
            Open,
            Stopped,
            Paused,
            Playing,
            Playing_Initial,
            Playing_History,
            Song1,
            Song2,
            Song3
        }

        enum Events {
            Undefined = 0,
            open_close,
            cd_detected,
            stop,
            play,
            pause,
            end_pause,
            NextSong,
            PreviousSong
        }

        class Context {
            public AbstractStates.StateMachine Controller
            {
                get;
                set;
            }

            public Context()
            {
                Log = new List<string>();
            }

            public List<String> Log
            {
                get;
                private set;
            }

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

            void OnAction(String p_Action, Event p_Event)
            {
                Events tE = Enum.IsDefined(typeof(States), p_Event.ID) ? (Events)p_Event.ID : Events.Undefined;
                Log.Add(String.Format("A: {0}, E: {1};", p_Action, tE.ToString()));
            }


            void OnAction(String p_Action, Event p_Event, String p_EventData)
            {
                Events tE = Enum.IsDefined(typeof(States), p_Event.ID) ? (Events)p_Event.ID : Events.Undefined;
                Log.Add(String.Format("A: {0}, E: {1}, D: {2};", p_Action, tE.ToString(), p_EventData));
            }

            public void open_drawer(Event p_Event)
            {
                OnAction("open_drawer", p_Event);
            }

            public void close_drawer(Event p_Event)
            {
                OnAction("close_drawer", p_Event);
            }

            public void store_cd_info(Event p_Event, String p_CDInfo)
            {
                OnAction("store_cd_info", p_Event, p_CDInfo);
            }

            public void stopped_again()
            {

            }

            public void stop_and_open()
            {

            }

            public void start_playback(Event p_Event)
            {
                OnAction("start_playback", p_Event);
            }

            public void stop_playback(Event p_Event)
            {
                OnAction("stop_playback", p_Event);
            }

            public void pause_playback(Event p_Event)
            {
                OnAction("pause_playback", p_Event);
            }

            public void resume_playback(Event p_Event)
            {
                OnAction("resume_playback", p_Event);
            }

            public void start_next_song(Event p_Event)
            {
                OnAction("start_next_song", p_Event);
            }

            public void start_prev_song(Event p_Event)
            {
                OnAction("start_prev_song", p_Event);
            }
        }

        void RegisterLog(Context p_Context)
        {
            foreach (var tS in p_Context.Controller) {
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
            p_Context.Controller.UnhandledEvent += m_Context.OnUnhandledEvent;
        }

        AbstractStates.StateMachine Create(Context p_Context)
        {
            var tB = new Builder();
            var Player = tB.StateMachine().ID(States.Player);
            var Player_Initial = tB.Initial(Player).ID(States.Player_Initial);
            var Empty = tB.Simple(Player).ID(States.Empty);
            var Open = tB.Simple(Player).ID(States.Open);
            var Stopped = tB.Simple(Player).ID(States.Stopped);
            var Paused = tB.Simple(Player).ID(States.Paused);
            var Playing = tB.Composite(Player).ID(States.Playing);
            var Playing_Initial = tB.Initial(Playing).ID(States.Playing_Initial);
            var Playing_History = tB.History(Playing, PseudoStates.HistoryKind.Shallow).ID(States.Playing_History);
            var Song1 = tB.Simple(Playing).ID(States.Song1);
            var Song2 = tB.Simple(Playing).ID(States.Song2);
            var Song3 = tB.Simple(Playing).ID(States.Song3);

            // transitions
            Player_Initial
                .AddTransition(Empty);
            Empty
                .AddTransition(Events.open_close, Open)
                    .Action(_ => m_Context.open_drawer(_.TriggeringEvent))
                .AddTransition(Events.cd_detected, Stopped)
                    .Action(_ => m_Context.store_cd_info(
                        _.TriggeringEvent, _.TriggeringEvent.Args.FirstOrDefault() as String));
            Stopped
                .AddTransition(Events.open_close, Open)
                    .Action(_ => m_Context.open_drawer(_.TriggeringEvent))
                .AddTransition(Events.stop)
                    .Kind(TransitionKind.External)
                    .Action(_ => m_Context.stopped_again())
                .AddTransition(Events.play, Playing)
                    .Action(_ => m_Context.start_playback(_.TriggeringEvent));
            Open
                .AddTransition(Events.open_close, Empty)
                .Action(_ => m_Context.close_drawer(_.TriggeringEvent));

            Paused
                .AddTransition(Events.open_close, Open)
                    .Action(_ => m_Context.stop_and_open())
                .AddTransition(Events.stop, Stopped)
                    .Action(_ => m_Context.stop_playback(_.TriggeringEvent))
                .AddTransition(Events.end_pause, Playing_History)
                    .Action(_ => m_Context.resume_playback(_.TriggeringEvent));

            Playing
                .AddTransition(Events.stop, Stopped)
                    .Action(_ => m_Context.stop_playback(_.TriggeringEvent))
                .AddTransition(Events.open_close, Open)
                    .Action(_ => m_Context.stop_and_open())
                .AddTransition(Events.pause, Paused)
                    .Action(_ => m_Context.pause_playback(_.TriggeringEvent));

            Playing_Initial
                .AddTransition(Song1);

            Playing_History
                .AddTransition(Song1);

            Song1
                .Enter(() => m_Context.Log.Add("Start Song1;"))
                .Exit(() => m_Context.Log.Add("Stop Song1;"))
                .AddTransition(Events.NextSong, Song2)
                    .Action(_ => m_Context.start_next_song(_.TriggeringEvent));
            Song2
                .Enter(() => m_Context.Log.Add("Start Song2;"))
                .Exit(() => m_Context.Log.Add("Stop Song2;"))
                .AddTransition(Events.PreviousSong, Song1)
                    .Action(_ => m_Context.start_prev_song(_.TriggeringEvent))
                .AddTransition(Events.NextSong, Song3)
                    .Action(_ => m_Context.start_next_song(_.TriggeringEvent));
            Song3
                .Enter(() => m_Context.Log.Add("Start Song3;"))
                .Exit(() => m_Context.Log.Add("Stop Song3;"))
                .AddTransition(Events.PreviousSong, Song2)
                    .Action(_ => m_Context.start_prev_song(_.TriggeringEvent));

            p_Context.Controller = Player;

            RegisterLog(p_Context);

            return Player;
        }

        public Boost_MSM_Player()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void SetUp()
        {
            m_Context = new Context();
            m_Context.Controller = Create(m_Context);
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void TeardDown()
        {
            m_Context = null;
        }

        #endregion

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
            var tH = AllStates[States.Playing_History] as PseudoStates.History;
            return tH.Count();
        }

        bool HistoryMustContain(Int32 p_Count)
        {
            var tH = AllStates[States.Playing_History] as PseudoStates.History;
            return tH.Count() == p_Count;
        }

        bool HistoryMustContain(States p_ExpectedHistory)
        {
            AbstractStates.State tState;
            var tH = AllStates[States.Playing_History] as PseudoStates.History;
            return tH.ToDictionary(_ => (States)_.ID).TryGetValue(p_ExpectedHistory, out tState);
        }

        bool HistoryMustContain(IEnumerable<States> p_ExpectedHistory)
        {
            var tH = AllStates[States.Playing_History] as PseudoStates.History;
            var tD = tH.ToDictionary(_ => (States)_.ID);
            bool tContains = true;
            foreach (var tS in p_ExpectedHistory) {
                AbstractStates.State tState;
                tContains &= tD.TryGetValue(tS, out tState);
            }

            return tContains;
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void Test_State_Tree_As_Expected()
        {
            CollectionAssert.AreEqual(new States[] {
                States.Player,
                States.Player_Initial,
                States.Empty,
                States.Open,
                States.Stopped,
                States.Paused,
                States.Playing,
                States.Playing_Initial,
                States.Playing_History,
                States.Song1,
                States.Song2,
                States.Song3
            }, AllStates.Select(_ => _.Key).ToArray());
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void Enter_Initial_Configuration()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Player_Initial-ENTER;",
                "Player_Initial-EXIT;",
                "Transition:Player_Initial->Empty;",
                "Empty-ENTER;"
            }, m_Context.Log);
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_Open_Close1()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Empty-EXIT;",
                "A: open_drawer, E: open_close;",
                "Transition:Empty->Open;",
                "Open-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_Open_Close2()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog();
            WriteHistoryStatus(tHistory);

            CollectionAssert.AreEqual(new String[] {
                "Open-EXIT;",
                "A: close_drawer, E: open_close;",
                "Transition:Open->Empty;",
                "Empty-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_cd_detected_louie_louie()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            CollectionAssert.AreEqual(new String[] {
                "Empty-EXIT;",
                "A: store_cd_info, E: cd_detected, D: louie, louie;",
                "Transition:Empty->Stopped;",
                "Stopped-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_play1()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            CollectionAssert.AreEqual(new String[] {
                "Stopped-EXIT;",
                "A: start_playback, E: play;",
                "Transition:Stopped->Playing;",
                "Playing-ENTER;",
                "Playing_Initial-ENTER;",
                "Playing_Initial-EXIT;",
                "Transition:Playing_Initial->Song1;",
                "Start Song1;",
                "Song1-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_NextSong1()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            CollectionAssert.AreEqual(new String[] {
                "Stop Song1;",
                "Song1-EXIT;",
                "A: start_next_song, E: NextSong;",
                "Transition:Song1->Song2;",
                "Start Song2;",
                "Song2-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_NextSong2()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song3].ID,
                (States)m_Context.Controller.Configuration.First().ID);


            CollectionAssert.AreEqual(new String[] {
                "Stop Song2;",
                "Song2-EXIT;",
                "A: start_next_song, E: NextSong;",
                "Transition:Song2->Song3;",
                "Start Song3;",
                "Song3-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_PreviousSong()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song3].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.PreviousSong));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);


            CollectionAssert.AreEqual(new String[] {
                "Stop Song3;",
                "Song3-EXIT;",
                "A: start_prev_song, E: PreviousSong;",
                "Transition:Song3->Song2;",
                "Start Song2;",
                "Song2-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_Pause()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song3].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.PreviousSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.pause));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Song2));
            Assert.AreEqual(
                (States)tAllStates[States.Paused].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            CollectionAssert.AreEqual(new String[] {
                "Stop Song2;",
                "Song2-EXIT;",
                "Playing-EXIT;",
                "A: pause_playback, E: pause;",
                "Transition:Playing->Paused;",
                "Paused-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_end_pause()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song3].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.PreviousSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.pause));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Song2));
            Assert.AreEqual(
                (States)tAllStates[States.Paused].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.end_pause));

            WriteLog();
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            CollectionAssert.AreEqual(new String[] {
                "Paused-EXIT;",
                "A: resume_playback, E: end_pause;",
                "Transition:Paused->Playing_History;",
                "Playing-ENTER;",
                "Playing_History-ENTER;",
                "Playing_History-EXIT;",
                "Start Song2;",
                "Song2-ENTER;",
            }, m_Context.Log);
            m_Context.Log.Clear();
        }

        [TestMethod]
        [TestProperty("Module", "Boost.MSM.Player")]
        public void MSM_Fire_pause_stop_stop_play()
        {
            var tAllStates = AllStates;
            var tHistory = tAllStates[States.Playing_History] as PseudoStates.History;
            m_Context.Log.Clear();

            m_Context.Controller.EnableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Open]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.open_close));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Empty]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.cd_detected, "louie, louie"));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            CollectionAssert.AreEqual(new AbstractStates.State[1] {
                tAllStates[States.Stopped]
            }, m_Context.Controller.Configuration.ToArray());

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.NextSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song3].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.PreviousSong));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.pause));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Song2));
            Assert.AreEqual(
                (States)tAllStates[States.Paused].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.end_pause));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song2].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.pause));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Song2));
            Assert.AreEqual(
                (States)tAllStates[States.Paused].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.stop));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Song2));
            Assert.AreEqual(
                (States)tAllStates[States.Stopped].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.stop));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(true, tHistory.IsHistoryActive);
            Assert.AreEqual(1, HistoryCount());
            Assert.AreEqual(true, HistoryMustContain(States.Song2));
            Assert.AreEqual(
                (States)tAllStates[States.Stopped].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.HandleEvent(Event.Create(Events.play));

            WriteLog(true);
            WriteHistoryStatus(tHistory);

            Assert.AreEqual(false, tHistory.IsHistoryActive);
            Assert.AreEqual(0, HistoryCount());
            Assert.AreEqual(
                (States)tAllStates[States.Song1].ID,
                (States)m_Context.Controller.Configuration.First().ID);

            m_Context.Controller.DisableEventHandling();

            WriteLog(true);
            WriteHistoryStatus(tHistory);
        }
    }
}
