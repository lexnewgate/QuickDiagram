using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming.Graph.Layout
{
    /// <summary>
    /// Publishes layout action events.
    /// </summary>
    internal interface IIncrementalLayoutEngine
    {
        void Clear();
        IEnumerable<ILayoutAction> GetLayoutActions(IEnumerable<DiagramShapeAction> diagramShapeActions);
    }
}