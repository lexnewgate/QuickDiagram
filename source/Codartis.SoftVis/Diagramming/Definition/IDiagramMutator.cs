﻿using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Diagramming.Definition
{
    /// <summary>
    /// Mutates a diagram.
    /// </summary>
    public interface IDiagramMutator
    {
        void AddNode(ModelNodeId nodeId, ModelNodeId? parentNodeId = null);
        void UpdateNodeHeaderSize(ModelNodeId nodeId, Size2D newSize);
        void UpdateNodeCenter(ModelNodeId nodeId, Point2D newCenter);
        void UpdateNodeTopLeft(ModelNodeId nodeId, Point2D newTopLeft);
        void RemoveNode(ModelNodeId nodeId);

        void AddConnector(ModelRelationshipId relationshipId);
        void UpdateConnectorRoute(ModelRelationshipId relationshipId, Route newRoute);
        void RemoveConnector(ModelRelationshipId relationshipId);

        /// <remarks>
        /// This should remove all shapes whose model ID does not exist in the new model.
        /// </remarks>
        void UpdateModel([NotNull] IModel newModel);

        void UpdateModelNode([NotNull] IModelNode updatedModelNode);

        void ApplyLayout([NotNull] GroupLayoutInfo diagramLayout);

        void Clear();
    }
}