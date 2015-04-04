using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Tests2 {
    public static class States {
        public enum Machine0 : int {
            Machine = 0,
            S0,
            S1,
            S2,
            S3,
            S4
        }

        public enum Machine1 : int {
            Machine = 0,
            S0,
            S1,
            S2,
            S3,
            S4,
            S5,
            S6,
            S7,
            S8,
            S9,
            S10,
            S11,
            S12,
            S13
        }

        public enum Samek : int {
            Samek = 0,
            Samek_Init,
            S,
            S1,
            S11,
            S2,
            S21,
            S211,
            S_Init,
            S1_Init,
            S2_Init,
            S21_Init,
            Samek_Term
        }
    }
}
