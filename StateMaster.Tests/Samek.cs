using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Core;

namespace StateMaster.Tests {
    public static class Samek {

        public static StateMaster.Core.Interfaces.IEventProcessor Create
        {
            // build state machine here
        }

        public static class S {

            public enum Events : int {
                A = 1, B, C, D, E, F, G, H
            }

            public const Int32 ID = 0;
            public static class Initial {
                public const Int32 ID = S.S2.ID;
            }

            public static class S1 {
                public const Int32 ID = 1;
                public static class Initial {
                    public const Int32 ID = S.S1.S11.ID;
                }

                public static class S11 {
                    public const Int32 ID = 11;
                }
            }

            public static class S2 {
                public const Int32 ID = 2;
                public static class Initial {
                    public const Int32 ID = S.S2.S21.S211.ID;
                }

                public static class S21 {
                    public const Int32 ID = 21;
                    public static class Initial {
                        public const Int32 ID = S.S2.S21.S211.ID;
                    }

                    public static class S211 {
                        public const Int32 ID = 211;
                    }
                }
            }
        }
    }
}
