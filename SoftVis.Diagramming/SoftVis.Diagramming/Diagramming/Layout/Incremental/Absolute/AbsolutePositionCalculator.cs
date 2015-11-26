﻿using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Geometry;
using MoreLinq;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Absolute
{
    /// <summary>
    /// Calculates vertex coordinates from relative layout info.
    /// </summary>
    internal class AbsolutePositionCalculator
    {
        private readonly IReadOnlyRelativeLayout _relativeLayout;
        private readonly double _horizontalGap;
        private readonly double _verticalGap;

        public AbsolutePositionCalculator(IReadOnlyRelativeLayout relativeLayout, 
            double horizontalGap, double verticalGap)
        {
            _relativeLayout = relativeLayout;
            _horizontalGap = horizontalGap;
            _verticalGap = verticalGap;
        }

        private IReadOnlyLayoutVertexLayers Layers => _relativeLayout.LayoutVertexLayers;

        public LayoutVertexToPointMap CalculateVertexCenters(Point2D startingPoint)
        {
            var vertexCenterXPositions = CalculateXPositions(startingPoint);
            var layerCenterYPositions = CalculateYPositions(_verticalGap);

            var vertexCenters = new LayoutVertexToPointMap();
            foreach (var layer in Layers)
            {
                var yPos = layerCenterYPositions[layer];
                foreach (var vertex in layer)
                {
                    var xPos = vertexCenterXPositions[vertex];
                    var center = new Point2D(xPos, yPos);
                    vertexCenters.Set(vertex, center);
                }
            }

            return vertexCenters;
        }

        private Dictionary<IReadOnlyLayoutVertexLayer, double> CalculateYPositions(double verticalGap)
        {
            var layerCenterYPositions = new Dictionary<IReadOnlyLayoutVertexLayer, double>();

            var previousBottom = -verticalGap;
            foreach (var layer in Layers)
            {
                var height = layer.Select(i => i.Height).Max();
                var bottom = previousBottom + verticalGap + height;

                layerCenterYPositions[layer] = bottom - height/2;
                previousBottom = bottom;
            }

            return layerCenterYPositions;
        }

        private Dictionary<LayoutVertexBase, double> CalculateXPositions(Point2D startingPoint)
        {
            var vertexCenterXPositions = new Dictionary<LayoutVertexBase, double>();

            foreach (var layer in Layers.Reverse())
            {
                var layerRight = double.MinValue;

                foreach (var vertex in layer)
                {
                    var vertexWidthHalf = vertex.Width / 2;
                    double xPos;

                    var children = _relativeLayout.ProperLayeredLayoutGraph.GetPrimaryChildren(vertex).ToList();
                    if (children.Any())
                    {
                        xPos = children.Select(i => vertexCenterXPositions[i]).Average();
                        var overlap = layerRight - (xPos - vertexWidthHalf - _horizontalGap);
                        if (overlap > 0)
                        {
                            xPos += overlap;
                            ShiftDescendants(vertex, overlap, vertexCenterXPositions);
                        }
                    }
                    else
                    {
                        xPos = Math.Max(layerRight, 0) + _horizontalGap + vertexWidthHalf;
                    }

                    vertexCenterXPositions[vertex] = xPos;
                    layerRight = xPos + vertexWidthHalf;
                }
            }

            //TODO: translate results according to the starting point

            return vertexCenterXPositions;
        }

        private void ShiftDescendants(LayoutVertexBase vertex, double shiftBy,
            Dictionary<LayoutVertexBase, double> vertexCenterXPositions)
        {
            _relativeLayout.ProperLayeredLayoutGraph.GetPrimaryDescendants(vertex)
                .ForEach(i => vertexCenterXPositions[i] += shiftBy);
        }
    }
}
