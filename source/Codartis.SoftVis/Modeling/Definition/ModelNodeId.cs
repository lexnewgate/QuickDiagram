﻿using System;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Identifies a model node through its lifetime.
    /// </summary>
    public struct ModelNodeId : IEquatable<ModelNodeId>, IComparable<ModelNodeId>
    {
        private readonly long _id;

        public ModelNodeId(long id)
        {
            _id = id;
        }

        public static ModelNodeId Parse([NotNull] string s) => new ModelNodeId(long.Parse(s));

        public override string ToString() => _id.ToString();

        public bool Equals(ModelNodeId other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is ModelNodeId && Equals((ModelNodeId)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(ModelNodeId left, ModelNodeId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModelNodeId left, ModelNodeId right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(ModelNodeId other)
        {
            return _id.CompareTo(other._id);
        }

        public static explicit operator long(ModelNodeId id) => id._id;
    }
}