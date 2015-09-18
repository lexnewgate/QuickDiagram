﻿using System.Collections.Generic;

namespace Codartis.SoftVis.Diagramming
{
    /// <summary>
    /// Represents a point in the diagram-space.
    /// </summary>
    public struct DiagramPoint
    {
        public double X { get; }
        public double Y { get; }

        public static DiagramPoint Zero = new DiagramPoint(0, 0);
        public static DiagramPoint Extreme = new DiagramPoint(double.NaN, double.NaN);

        public DiagramPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static bool Equals(DiagramPoint point1, DiagramPoint point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y);
        }

        public bool Equals(DiagramPoint other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DiagramPoint)) return false;

            var value = (DiagramPoint)obj;
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"( {X} , {Y} )";
        }

        public static bool operator ==(DiagramPoint left, DiagramPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiagramPoint left, DiagramPoint right)
        {
            return !left.Equals(right);
        }

        public static DiagramPoint operator +(DiagramPoint left, DiagramPoint right)
        {
            return new DiagramPoint(left.X + right.X, left.Y + right.Y);
        }

        public static DiagramPoint operator -(DiagramPoint left, DiagramPoint right)
        {
            return new DiagramPoint(left.X - right.X, left.Y - right.Y);
        }

        public static DiagramPoint operator *(DiagramPoint point, double multiplier)
        {
            return new DiagramPoint(point.X * multiplier, point.Y * multiplier);
        }

        public static DiagramPoint operator *(double multiplier, DiagramPoint point)
        {
            return point * multiplier;
        }

        public static DiagramPoint[] CreateRoute(DiagramPoint firstPoint, DiagramPoint secondPoint, DiagramPoint? thirdPoint,
            IEnumerable<DiagramPoint> internalPoints, DiagramPoint? beforePenultimatePoint, DiagramPoint penultimatePoint, DiagramPoint lastPoint)
        {
            var route = new List<DiagramPoint>();

            route.Add(firstPoint);
            route.Add(secondPoint);
            if (thirdPoint!=null) route.Add(thirdPoint.Value);

            if (internalPoints != null)
                route.AddRange(internalPoints);

            if (beforePenultimatePoint != null) route.Add(beforePenultimatePoint.Value);
            route.Add(penultimatePoint);
            route.Add(lastPoint);

            return route.ToArray();
        }

        public static DiagramPoint[] CreateRoute(DiagramPoint firstPoint, IEnumerable<DiagramPoint> internalPoints, DiagramPoint lastPoint)
        {
            var route = new List<DiagramPoint>();

            route.Add(firstPoint);

            if (internalPoints != null)
                route.AddRange(internalPoints);

            route.Add(lastPoint);

            return route.ToArray();
        }
    }
}
