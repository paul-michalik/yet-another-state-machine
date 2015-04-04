using System;
using System.Collections.Generic;
using System.Linq;
using StateMaster.Core.Interfaces;

namespace StateMaster.Core {

    public static class LCASearch {

        public class Result {
            readonly State m_LCA;
            readonly IEnumerable<State> m_SourcePath, m_TargetPath;

            public Result() {}

            public Result(State p_LCA, IEnumerable<State> p_SourcePath, IEnumerable<State> p_TargetPath)
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

            public IEnumerable<State> PathFromSourceToLCA 
            { 
                get { return m_SourcePath; } 
            }

            public IEnumerable<State> PathFromLCAToTarget 
            { 
                get { return m_TargetPath; } 
            }

            public State LCA 
            { 
                get { return m_LCA; } 
            }
        }

        /// <summary>
        /// Construct the Result of the Leat Common Ancestor search
        /// </summary>
        /// <param name="p_Source">Source State</param>
        /// <param name="p_Target">Target State</param>
        /// <returns>LCASearch.Result instance</returns>
        public static Result Execute(State p_Source, State p_Target)
        {
            var tSourcePath = LCASearch.ConstructPathToRoot(p_Source);
            var tTargetPath = LCASearch.ConstructPathToRoot(p_Target);

            var tLCA = ConstructPaths(tSourcePath, tTargetPath);
            return new LCASearch.Result(tLCA, tSourcePath, tTargetPath);
        }

        internal static void RemoveFromNextToEnd(Int32 p_LCAIndex, List<State> p_Path)
        {
            Int32 tLastIndex = p_Path.Count - 1;
            if (p_LCAIndex + 1 <= tLastIndex)
                p_Path.RemoveRange(p_LCAIndex + 1, tLastIndex - p_LCAIndex);
        }

        internal static bool PathsAreValid(List<State> p_SourcePath, List<State> p_TargetPath)
        {
            return p_SourcePath.Count > 0 && p_TargetPath.Count > 0;
        }

        internal static List<State> ConstructPathToRoot(State p_State)
        {
            var tPath = new List<State>();
            while (p_State != null) {
                tPath.Add(p_State);
                p_State = p_State.Parent;
            }
            return tPath;
        }

        internal static State ConstructPaths(List<State> p_SourcePath, List<State> p_TargetPath)
        {
            State tLCA = null;
            // search first different State in paths walking from root:
            Int32 tLCAIndexS = p_SourcePath.Count - 1,
                tLCAIndexT = p_TargetPath.Count - 1;
            for (; tLCAIndexS >= 0 && tLCAIndexT >= 0; tLCAIndexS--, tLCAIndexT--) {
                if (p_SourcePath[tLCAIndexS] == p_TargetPath[tLCAIndexT]) {
                    tLCA = p_SourcePath[tLCAIndexS];
                } else {
                    break;
                }
            }

            // Otherwise States must stem from different State trees.
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
