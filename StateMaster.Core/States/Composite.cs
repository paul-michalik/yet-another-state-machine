using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Core.States {

    class Composite : State {

        internal IList<Region> Regions { get; set; }
        internal State History { get; set; }
        internal State Initial { get; set; }

        internal Composite() {}

        public override void Enter()
        {
            base.Enter();
            if (History != null) {

            } else if (Initial != null) {

            }
        }
        public override void Exit()
        {
            if (History != null) {

            }
            base.Exit();
        }
    }
}
