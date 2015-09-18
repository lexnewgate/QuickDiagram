using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama
{
    internal class EdgeRoutingAlgorithm : IEdgeRoutingAlgorithm<IExtent, IEdge<IExtent>> 
    {
        private readonly IEnumerable<IEdge<IExtent>> _originalEdges;
        private readonly SugiGraph _sugiGraph;
        private readonly SugiyamaLayoutParameters _layoutParameters;
        private readonly Layers _layers;
        private readonly EdgeToDummyVerticesMap _edgeToDummyVerticesMap;

        public IDictionary<IEdge<IExtent>, DiagramPoint[]> EdgeRoutes { get; private set; }

        public EdgeRoutingAlgorithm(IEnumerable<IEdge<IExtent>> originalEdges, SugiGraph sugiGraph, 
            SugiyamaLayoutParameters layoutParameters, Layers layers, EdgeToDummyVerticesMap edgeToDummyVerticesMap)
        {
            _originalEdges = originalEdges;
            _sugiGraph = sugiGraph;
            _layoutParameters = layoutParameters;
            _layers = layers;
            _edgeToDummyVerticesMap = edgeToDummyVerticesMap;
        }

        public void Compute()
        {
            var edgeRoutingType = _layoutParameters.EdgeRoutingType;

            switch (edgeRoutingType)
            {
                case EdgeRoutingType.Straight:
                    EdgeRoutes = CalculateStraightEdgeRouting();
                    break;
                case EdgeRoutingType.Orthogonal:
                    EdgeRoutes = CalculateOrthogonalEdgeRouting();
                    break;
                default:
                    throw new Exception($"Unexpected EdgeRoutingType: {edgeRoutingType}");
            }
        }

        private Dictionary<IEdge<IExtent>, DiagramPoint[]> CalculateStraightEdgeRouting()
        {
            var edgeRoutes = new Dictionary<IEdge<IExtent>, DiagramPoint[]>();

            foreach (var edge in _originalEdges)
            {
                var internalRoutePoints = _edgeToDummyVerticesMap.GetRoutePoints(edge)?.ToList();

                var secondPoint = internalRoutePoints?.First() ?? NewCenter(edge.Target);
                var firstPoint = NewRect(edge.Source).GetAttachPointToward(secondPoint);

                var penultimatePoint = internalRoutePoints?.Last() ?? NewCenter(edge.Source);
                var lastPoint = NewRect(edge.Target).GetAttachPointToward(penultimatePoint);

                edgeRoutes.Add(edge, DiagramPoint.CreateRoute(firstPoint, internalRoutePoints, lastPoint));
            }

            return edgeRoutes;
        }

        private Dictionary<IEdge<IExtent>, DiagramPoint[]> CalculateOrthogonalEdgeRouting()
        {
            var edgeRoutes = new Dictionary<IEdge<IExtent>, DiagramPoint[]>();

            var layerDistance = _layoutParameters.LayerDistance;

            foreach (var edge in _originalEdges)
            {
                var sourceVertex = _sugiGraph.GetSugiVertexByOriginal(edge.Source);
                var targetVertex = _sugiGraph.GetSugiVertexByOriginal(edge.Target);

                var sourceLayer = _layers[sourceVertex];
                var targetLayer = _layers[targetVertex];

                var isUpsideDown = sourceVertex.LayerIndex > targetVertex.LayerIndex;

                var sourceVertical = isUpsideDown
                    ? sourceLayer.Position - layerDistance / 2.0
                    : sourceLayer.Position + sourceLayer.Height + layerDistance / 2.0;

                var secondPoint = new DiagramPoint(sourceVertex.HorizontalPosition, sourceVertical);
                var firstPoint = NewRect(edge.Source).GetAttachPointToward(secondPoint);

                var targetVertical = isUpsideDown
                    ? targetLayer.Position + targetLayer.Height + layerDistance / 2.0
                    : targetLayer.Position - layerDistance / 2.0;

                var penultimatePoint = new DiagramPoint(targetVertex.HorizontalPosition, targetVertical);
                var lastPoint = NewRect(edge.Target).GetAttachPointToward(penultimatePoint);

                var dummyVertexPoints = _edgeToDummyVerticesMap[edge]?.Select(i => new DiagramPoint(i.HorizontalPosition, i.VerticalPosition)).ToList();

                DiagramPoint? thirdPoint = null;
                DiagramPoint? beforePenultimatePoint = null;
                if (dummyVertexPoints != null)
                {
                    thirdPoint = new DiagramPoint(dummyVertexPoints.First().X, secondPoint.Y);
                    beforePenultimatePoint = new DiagramPoint(dummyVertexPoints.Last().X, penultimatePoint.Y);
                }

                var route = DiagramPoint.CreateRoute(firstPoint, secondPoint, thirdPoint, dummyVertexPoints,
                    beforePenultimatePoint, penultimatePoint, lastPoint);
                route = RemoveConsecutiveSamePoints(route);

                edgeRoutes.Add(edge, route);
            }

            return edgeRoutes;
        }

        private static DiagramPoint[] RemoveConsecutiveSamePoints(DiagramPoint[] route)
        {
            var resultPoints = new List<DiagramPoint>();

            var previousPoint = DiagramPoint.Extreme;
            foreach (var point in route)
            {
                if (point != previousPoint)
                    resultPoints.Add(point);
                
                previousPoint = point;
            }

            return resultPoints.ToArray();
        }

        private DiagramPoint NewCenter(IExtent vertex)
        {
            return _sugiGraph.GetNewCenter(vertex);
        }

        private DiagramRect NewRect(IExtent vertex)
        {
            var newCenter = NewCenter(vertex);
            var size = vertex.Size;
            return DiagramRect.CreateFromCenterAndSize(newCenter, size);
        }
    }
}