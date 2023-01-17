// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;
using GF.Base;

namespace TnyFramework.Navmesh.Navmesh.Collection
{

    public class BvhTree2<TValue>
    {
        private FmVector2 min;
        private FmVector2 max;

        private readonly Fix64 quotaFactor;

        private BvhNode2<TValue>[] nodes;

        private static readonly IComparer<BvhItem2<TValue>> X_COMPARER = new BvhItemXComparer<BvhItem2<TValue>>();
        private static readonly IComparer<BvhItem2<TValue>> Y_COMPARER = new BvhItemYComparer<BvhItem2<TValue>>();

        public BvhTree2(FmVector2 min, FmVector2 max, Fix64 quotaFactor)
        {
            this.min = min;
            this.quotaFactor = quotaFactor;
            this.max = max;
        }

        public void Create<TElement>(TElement[] elements, IBvhElementVisitor<TElement, TValue> visitor)
        {
            var items = new BvhItem2<TValue>[elements.Length];
            var count = elements.Length;
            for (var index = 0; index < count; index++)
            {
                var element = elements[index];
                var eMin = visitor.GetMin(element, index);
                var eMax = visitor.GetMax(element, index);
                var value = visitor.GetValue(element, index);

                QuotaVector2 bMin = default;
                QuotaVector2 bMax = default;

                // BV-tree uses cs for all dimensions
                bMin.X = Clamp((int) ((eMin.X - min.X) * quotaFactor), 0, 0x7fffffff);
                bMin.Y = Clamp((int) ((eMin.Y - min.Y) * quotaFactor), 0, 0x7fffffff);

                bMax.X = Clamp((int) ((eMax.X - min.X) * quotaFactor), 0, 0x7fffffff);
                bMax.Y = Clamp((int) ((eMax.Y - min.Y) * quotaFactor), 0, 0x7fffffff);

                items[index] = new BvhItem2<TValue>(bMin, bMax, index, value);
            }
            nodes = new BvhNode2<TValue>[items.Length * 2];
            Subdivide(items, 0, count, 0);
            nodes[nodes.Length - 1] = BvhNode2<TValue>.NoneOf();
        }

        public List<TValue> Query(FmVector2 point)
        {
            return Query(point, point);
        }

        public List<TValue> Query(FmVector2 queryMin, FmVector2 queryMax)
        {

            var minX = Clamp(queryMin.X, min.X, max.X) - min.X;
            var minY = Clamp(queryMin.Y, min.Y, max.Y) - min.Y;
            var maxX = Clamp(queryMax.X, min.X, max.X) - min.X;
            var maxy = Clamp(queryMax.Y, min.Y, max.Y) - min.Y;

            // Quantize
            var queryBoundMin = new QuotaVector2((int) (quotaFactor * minX) & 0x7ffffffe, (int) (quotaFactor * minY) & 0x7ffffffe);
            var queryBoundMax = new QuotaVector2((int) (quotaFactor * maxX + 1) | 1, (int) (quotaFactor * maxy + 1) | 1);

            var values = new List<TValue>();

            var index = 0;
            var maxSize = nodes.Length;
            while (index < maxSize)
            {
                var bvhNode = nodes[index];
                if (bvhNode.None)
                {
                    break;
                }
                var overlap = OverlapQuotaBounds(queryBoundMin, queryBoundMax, bvhNode);
                var leaf = bvhNode.Leaf;
                if (leaf && overlap)
                {
                    values.Add(bvhNode.Value);
                }

                if (overlap || leaf)
                {
                    index++;
                } else
                {
                    index = bvhNode.Next;
                }
            }
            return values;
        }

        private static bool OverlapQuotaBounds(QuotaVector2 queryBoundMin, QuotaVector2 queryBoundMax, BvhNode2<TValue> bvhNode)
        {
            var overlap = true;
            overlap = queryBoundMin.X <= bvhNode.Max.X && queryBoundMax.X >= bvhNode.Min.X;
            overlap = queryBoundMin.Y <= bvhNode.Max.Y && queryBoundMax.Y >= bvhNode.Min.Y && overlap;
            return overlap;
        }

        private int Subdivide(BvhItem2<TValue>[] items, int start, int end, int currentNode)
        {
            var count = end - start;
            var nodeIndex = currentNode;

            if (count == 1)
            {
                // Leaf
                nodes[nodeIndex++] = new BvhNode2<TValue>(items[start]);
            } else
            {
                var bound = CalculateExtends(items, start, end);
                var currentIndex = nodeIndex++;
                var boundMin = bound.Item1;
                var boundMax = bound.Item2;

                var axis = LongestAxis(boundMin, boundMax);
                Array.Sort(items, start, count, axis == 0 ? X_COMPARER : Y_COMPARER);

                var splitIndex = start + count / 2;

                // Left
                nodeIndex = Subdivide(items, start, splitIndex, nodeIndex);
                // Right
                nodeIndex = Subdivide(items, splitIndex, end, nodeIndex);

                nodes[currentIndex] = new BvhNode2<TValue>(bound.Item1, bound.Item2, nodeIndex);
            }
            return nodeIndex;
        }

        private static Tuple<QuotaVector2, QuotaVector2> CalculateExtends(BvhItem2<TValue>[] items, int start, int end)
        {
            var min = items[start].Min;
            var max = items[start].Max;

            for (var i = start + 1; i < end; ++i)
            {
                var it = items[i];
                if (it.Min.X < min.X)
                    min.X = it.Min.X;
                if (it.Min.Y < min.Y)
                    min.Y = it.Min.Y;

                if (it.Max.X > max.X)
                    max.X = it.Max.X;
                if (it.Max.Y > max.Y)
                    max.Y = it.Max.Y;
            }
            return new Tuple<QuotaVector2, QuotaVector2>(min, max);
        }

        private static int LongestAxis(IQuotaVector2 min, IQuotaVector2 max)
        {
            var xLenght = Math.Abs(max.X - min.X);
            var yLenght = Math.Abs(max.Y - min.Y);
            return yLenght > xLenght ? 1 : 0;
        }

        private int Clamp(int value, int minValue, int maxValue)
        {
            return Math.Max(Math.Min(value, maxValue), minValue);
        }

        private Fix64 Clamp(Fix64 value, Fix64 minValue, Fix64 maxValue)
        {
            var temp = (value <= maxValue) ? value : maxValue;
            return temp >= minValue ? temp : minValue;
        }
    }

    internal class BvhItemXComparer<TItem> : IComparer<TItem>
        where TItem : IBvhItem2
    {
        public int Compare(TItem one, TItem other)
        {
            if (one == null && other == null)
            {
                return 0;
            }
            if (one == null)
            {
                return -1;
            }
            if (other == null)
            {
                return 1;
            }
            var oneValue = one.Min.X;
            var otherValue = other.Min.X;
            return (oneValue < otherValue) ? -1 : (oneValue == otherValue ? 0 : 1);
        }
    }

    internal class BvhItemYComparer<TItem> : IComparer<TItem>
        where TItem : IBvhItem2
    {
        public int Compare(TItem one, TItem other)
        {
            if (one == null && other == null)
            {
                return 0;
            }
            if (one == null)
            {
                return -1;
            }
            if (other == null)
            {
                return 1;
            }
            var oneValue = one.Min.Y;
            var otherValue = other.Min.Y;
            return (oneValue < otherValue) ? -1 : (oneValue == otherValue ? 0 : 1);
        }
    }

}
