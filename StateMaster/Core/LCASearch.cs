using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateMaster.Extensions;
namespace StateMaster.Core {
    public static class LCASearch {

        public class Result {
            readonly AbstractStates.State m_LCA;
            readonly IEnumerable<AbstractStates.State> m_SourcePath, m_TargetPath;

            Result() {}

            public Result(
                AbstractStates.State p_LCA, 
                IEnumerable<AbstractStates.State> p_SourcePath, 
                IEnumerable<AbstractStates.State> p_TargetPath)
            {
                m_LCA = p_LCA;
                m_SourcePath = p_SourcePath;
                m_TargetPath = p_TargetPath;
            }

            public bool Valid
            {
                get
                {
                    return m_SourcePath != null && m_TargetPath != null;
                }
            }

            public IEnumerable<AbstractStates.State> PathFromSourceToLCA
            {
                get { return m_SourcePath; }
            }

            public IEnumerable<AbstractStates.State> PathFromLCAToTarget
            {
                get { return m_TargetPath; }
            }

            public AbstractStates.State LCA
            {
                get { return m_LCA; }
            }
        }

        /// <summary>
        /// Construct the Result of the Leat Common Ancestor search
        /// </summary>
        /// <param name="p_Source">Source AbstractStates.StateBuilder</param>
        /// <param name="p_Target">Target AbstractStates.StateBuilder</param>
        /// <returns>LCASearch.Result instance</returns>
        public static Result Execute(AbstractStates.State p_Source, AbstractStates.State p_Target)
        {
            var tSourcePath = LCASearch.ConstructPathToRoot(p_Source);
            var tTargetPath = LCASearch.ConstructPathToRoot(p_Target);

            var tLCA = ConstructPaths(tSourcePath, tTargetPath);
            return new LCASearch.Result(tLCA, tSourcePath, tTargetPath);
        }

        struct Pair<T1, T2> {
            public T1 First { get; set; }
            public T2 Second { get; set; }
        }

        static Pair<T1, T2> Create<T1, T2>(T1 p_First, T2 p_Second)
        {
            return new Pair<T1, T2> {
                First = p_First,
                Second = p_Second
            };
        }

        public static AbstractStates.State FindLCA(
            AbstractStates.State p_Source, AbstractStates.State p_Target)
        {
            var tSourcePath = p_Source.PathToParent().Reverse();
            var tTargetPath = p_Target.PathToParent().Reverse();
            
            return tSourcePath
                .Zip(tTargetPath, (_1, _2) => LCASearch.Create(_1, _2))
                .LastOrDefault(_ => Object.ReferenceEquals(_.First, _.Second)).First;
        }

        public static AbstractStates.State FindLCA(IEnumerable<AbstractStates.State> p_States)
        {
            var tLCA = p_States.First();
            foreach (var tS in p_States.Skip(1)) {
                tLCA = FindLCA(tLCA, tS);
            }
            return tLCA;
        }

        internal static void RemoveFromNextToEnd(Int32 p_LCAIndex, List<AbstractStates.State> p_Path)
        {
            Int32 tLastIndex = p_Path.Count - 1;
            if (p_LCAIndex + 1 <= tLastIndex)
                p_Path.RemoveRange(p_LCAIndex + 1, tLastIndex - p_LCAIndex);
        }

        internal static bool PathsAreValid(
            List<AbstractStates.State> p_SourcePath, 
            List<AbstractStates.State> p_TargetPath)
        {
            return p_SourcePath.Count > 0 && p_TargetPath.Count > 0;
        }

        internal static List<AbstractStates.State> ConstructPathToRoot(AbstractStates.State p_State)
        {
            var tPath = new List<AbstractStates.State>();
            while (p_State != null) {
                tPath.Add(p_State);
                p_State = p_State.Parent;
            }
            return tPath;
        }

        internal static AbstractStates.State ConstructPaths(
            List<AbstractStates.State> p_SourcePath, 
            List<AbstractStates.State> p_TargetPath)
        {
            AbstractStates.State tLCA = null;
            // search first different AbstractStates.StateBuilder in paths walking from root:
            Int32 
                tLCAIndexS = p_SourcePath.Count - 1,
                tLCAIndexT = p_TargetPath.Count - 1;
            for (; tLCAIndexS >= 0 && tLCAIndexT >= 0; tLCAIndexS--, tLCAIndexT--) {
                if (p_SourcePath[tLCAIndexS] == p_TargetPath[tLCAIndexT]) {
                    tLCA = p_SourcePath[tLCAIndexS];
                } else {
                    break;
                }
            }

            // Otherwise States must stem from different AbstractStates.StateBuilder trees.
            // In that case the complete paths from source to root and from
            // root to target are stored
            if (tLCA != null) {
                // keep only States from node to LCA
                tLCAIndexS = tLCAIndexS + 1;
                LCASearch.RemoveFromNextToEnd(tLCAIndexS, p_SourcePath);

                tLCAIndexT = tLCAIndexT + 1;
                LCASearch.RemoveFromNextToEnd(tLCAIndexT, p_TargetPath);
            }

            // reverse the order of States in entry set:
            p_TargetPath.Reverse();
            return tLCA;
        }
    }
}
