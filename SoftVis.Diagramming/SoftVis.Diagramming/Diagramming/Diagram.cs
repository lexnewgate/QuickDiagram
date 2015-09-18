﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Diagramming.Graph.Layout;
using Codartis.SoftVis.Diagramming.Graph.Layout.EfficientSugiyama;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// A diagram is a partial, graphical representation of a model. 
    /// A diagram shows a subset of the model and there can be many diagrams depicting different areas/aspects of the same model.
    /// A diagram consists of shapes that represent model elements.
    /// The shapes form a directed graph: some shapes are nodes in the graph and others are connectors between nodes.
    /// The layout of the shapes (relative positions and size) also conveys meaning.
    /// </summary>
    [DebuggerDisplay("VertexCount={_graph.VertexCount}, EdgeCount={_graph.EdgeCount}")]
    public abstract class Diagram
    {
        protected static readonly DiagramPoint DefaultNodePosition = DiagramPoint.Zero;
        protected static readonly DiagramSize DefaultNodeSize = new DiagramSize(100,38);

        private readonly DiagramGraph _graph = new DiagramGraph();

        public IEnumerable<DiagramNode> Nodes => _graph.Vertices;
        public IEnumerable<DiagramConnector> Connectors => _graph.Edges;

        public event EventHandler<DiagramShape> ShapeAdded;
        public event EventHandler<DiagramShape> ShapeModified;
        public event EventHandler<DiagramShape> ShapeRemoved;
        public event EventHandler<DiagramShape> ShapeSelected;
        public event EventHandler<DiagramShape> ShapeActivated;
        public event EventHandler Cleared;

        /// <summary>
        /// Show a node on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        public void ShowNode(IModelEntity modelEntity)
        {
            if (!NodeExists(modelEntity))
            {
                var node = CreateDiagramNode(modelEntity);
                _graph.AddVertex(node);
                OnShapeAdded(node);
            }

            foreach (var modelRelationship in modelEntity.OutgoingRelationships.Concat(modelEntity.IncomingRelationships))
            {
                if (NodeExists(modelRelationship.Source) &&
                    NodeExists(modelRelationship.Target))
                {
                    ShowConnector(modelRelationship);
                }
            }
        }

        /// <summary>
        /// Show a connector on the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A relationship model item.</param>
        public void ShowConnector(IModelRelationship modelRelationship)
        {
            if (ConnectorExists(modelRelationship))
                return;

            var connector = CreateDiagramConnector(modelRelationship);
            _graph.AddEdge(connector);
            OnShapeAdded(connector);
        }

        /// <summary>
        /// Hide a node from the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelEntity">A type or package model element.</param>
        public void HideNode(IModelEntity modelEntity)
        {
            if (!NodeExists(modelEntity))
                return;

            var node = FindNode(modelEntity);
            _graph.RemoveVertex(node);
            OnShapeRemoved(node);
        }

        /// <summary>
        /// Hodes a connector from the diagram that represents the given model element.
        /// </summary>
        /// <param name="modelRelationship">A modelRelationship model item.</param>
        public void HideConnector(IModelRelationship modelRelationship)
        {
            if (!ConnectorExists(modelRelationship))
                return;

            var connector = FindConnector(modelRelationship);
            _graph.RemoveEdge(connector);
            OnShapeRemoved(connector);
        }

        /// <summary>
        /// Recalculates the layout of the diagram and applies the new shape positions and edge routes.
        /// </summary>
        public void Layout(LayoutType layoutType, ILayoutParameters layoutParameters = null)
        {
            switch (layoutType)
            {
                case (LayoutType.Tree):
                    ApplySimpleTreeLayoutAndStraightEdgeRouting();
                    break;
                case (LayoutType.Sugiyama):
                    ApplySugiyamaLayoutAndRouting(layoutParameters);
                    break;
                default:
                    throw new ArgumentException($"Unexpected layout type: {layoutType}");
            }
        }

        /// <summary>
        /// Clear the diagram (that is, hides all nodes and connectors).
        /// </summary>
        public void Clear()
        {
            _graph.Clear();
            OnCleared();
        }

        protected abstract DiagramNode CreateDiagramNode(IModelEntity modelEntity);

        protected abstract DiagramConnector CreateDiagramConnector(IModelRelationship relationship);

        private void ApplySimpleTreeLayoutAndStraightEdgeRouting()
        {
            var layoutAlgorithm = new SimpleTreeLayoutAlgorithm<DiagramNode, DiagramConnector>(_graph);
            layoutAlgorithm.Compute();

            ApplyVertexCenters(layoutAlgorithm.VertexCenters);

            var routingAlgorithm = new StraightEdgeRoutingAlgorithm<DiagramNode, DiagramConnector>(_graph);
            routingAlgorithm.Compute();

            ApplyConnectorRoutes(routingAlgorithm.EdgeRoutes);
        }

        private void ApplySugiyamaLayoutAndRouting(ILayoutParameters layoutParameters)
        {
            var algorithm = new SugiyamaLayoutAlgorithm<DiagramNode, DiagramConnector>(_graph, (SugiyamaLayoutParameters)layoutParameters);
            algorithm.Compute();

            ApplyVertexCenters(algorithm.VertexCenters);
            ApplyConnectorRoutes(algorithm.EdgeRoutes);
        }

        private void ApplyVertexCenters(IDictionary<DiagramNode, DiagramPoint> vertexCenters)
        {
            foreach (var node in Nodes)
            {
                node.Center = vertexCenters[node];
                OnShapeModified(node);
            }
        }

        private void ApplyConnectorRoutes(IDictionary<DiagramConnector, DiagramPoint[]> edgeRoutes)
        {
            foreach (var connector in Connectors)
            {
                connector.RoutePoints = edgeRoutes[connector];
                OnShapeModified(connector);
            }
        }

        protected DiagramNode FindNode(IModelEntity modelEntity)
        {
            return Nodes.FirstOrDefault(i => Equals(i.ModelEntity, modelEntity));
        }

        private bool NodeExists(IModelEntity modelEntity)
        {
            return Nodes.Any(i => Equals(i.ModelEntity, modelEntity));
        }

        private DiagramConnector FindConnector(IModelRelationship modelRelationship)
        {
            return Connectors.FirstOrDefault(i => Equals(i.ModelRelationship, modelRelationship));
        }

        private bool ConnectorExists(IModelRelationship modelRelationship)
        {
            return Connectors.Any(i => Equals(i.ModelRelationship, modelRelationship));
        }

        private void OnShapeAdded(DiagramShape shape)
        {
            ShapeAdded?.Invoke(this, shape);
        }

        private void OnShapeModified(DiagramShape shape)
        {
            ShapeModified?.Invoke(this, shape);
        }

        private void OnShapeRemoved(DiagramShape shape)
        {
            ShapeRemoved?.Invoke(this, shape);
        }

        private void OnCleared()
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        public void OnShapeSelected(DiagramShape diagramShape)
        {
            ShapeSelected?.Invoke(this, diagramShape);
        }

        public void OnShapeActivated(DiagramShape diagramShape)
        {
            ShapeActivated?.Invoke(this, diagramShape);
        }
    }
}
