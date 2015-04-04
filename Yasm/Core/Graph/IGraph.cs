using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yasm.Core.Graph {

    public interface INode<out TVertex, out TEdge> : IElement
        where TVertex : INode<TVertex, TEdge>
        where TEdge : IEdge<TVertex, TEdge> 
    {
        IEnumerable<TEdge> Outgoing { get; }
        IEnumerable<TEdge> Incoming { get; }
        TVertex Element { get; }
    }

    public interface IEdge<out TVertex, out TEdge> : IElement
        where TVertex : INode<TVertex, TEdge> 
        where TEdge : IEdge<TVertex, TEdge>
    {
        TVertex Source { get; }
        TVertex Target { get; }
    }

    public interface IGraph<out TVertex, out TEdge>
        where TVertex : INode<TVertex, TEdge> 
        where TEdge : IEdge<TVertex, TEdge>
    {
        IEnumerable<TVertex> Vertices { get; }
        IEnumerable<TEdge> Edges { get; }
    }
}
