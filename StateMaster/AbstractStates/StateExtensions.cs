using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMaster.Extensions {
    public static class StateExtensions {

        public static IEnumerable<Core.Transition> Incoming(this AbstractStates.State p_State)
        {
            var tTop = p_State.PathToParent()
                .Last() as AbstractStates.Composite;
            if (tTop != null) {
                foreach (var tT in tTop
                    .Where(pS => pS.Transitions != null)
                    .SelectMany(pS => pS.Transitions.Select(pP => pP.Value))
                    .Where(pP => pP.Target == p_State)) {
                    yield return tT;
                }
            }
        }

        public static IEnumerable<Core.Transition> Outgoing(this AbstractStates.State p_State)
        {
            var tTransitions = p_State.Transitions;
            if (tTransitions != null) {
                foreach (var tT in p_State.Transitions.Select(pT => pT.Value)) {
                    yield return tT;
                }
            }
        }

        /// <summary>
        /// Returns a path to given parent node, including p_This and the parent.
        /// </summary>
        /// <param name="p_This">Start node</param>
        /// <param name="p_Parent">End of the path, root if null</param>
        /// <returns>Path to parent</returns>
        public static IEnumerable<AbstractStates.State> PathToParent(
            this AbstractStates.State p_This, AbstractStates.State p_Parent = null)
        {
            if (p_This != null) {
                var tS = p_This;
                for (; tS != p_Parent; tS = tS.Parent) {
                    yield return tS;
                }
                if (tS != null && tS == p_Parent)
                    yield return tS;
            }
        }

        public static IEnumerable<AbstractStates.State> Expand(this AbstractStates.Composite p_This)
        {
            if (p_This != null) {
                var tComposite = p_This;
                var tChildren = tComposite.Children;
                if (tChildren != null) {
                    foreach (var tC in tChildren) {
                        yield return tC;
                    }
                }
            }
        }

        public static IEnumerable<AbstractStates.State> Expand(this AbstractStates.State p_This)
        {
            if (p_This != null) {
                var tComposite = p_This as AbstractStates.Composite;
                if (tComposite != null) {
                    foreach (var tS in tComposite.Expand())
                        yield return tS;
                }
            }
        }

        public static IEnumerable<AbstractStates.State> TraverseBreadthFirst(this AbstractStates.State p_This)
        {
            if (p_This != null) {
                var tQ = new Queue<AbstractStates.State>();
                tQ.Enqueue(p_This);
                while (tQ.Count > 0) {
                    var tS1 = tQ.Dequeue();
                    yield return tS1;
                    foreach (var tS2 in tS1.Expand()) {
                        tQ.Enqueue(tS2);
                    }
                }
            }
        }

        public static IEnumerable<AbstractStates.State> TraversePostOrder(this AbstractStates.State p_This)
        {
            if (p_This != null) {
                foreach (var tS1 in p_This.Expand()) {
                    foreach (var tS2 in tS1.TraversePostOrder()) {
                        yield return tS2;
                    }
                }
                yield return p_This;
            }
        }

        public static IEnumerable<AbstractStates.State> TraversePreOrder(this AbstractStates.State p_This)
        {
            if (p_This != null) {
                yield return p_This;
                foreach (var tS1 in p_This.Expand()) {
                    foreach (var tS2 in tS1.TraversePreOrder()) {
                        yield return tS2;
                    }
                }
            }
        }
    }
}
