﻿#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public static class BlockRendererIterator
    {
        private static void add_intersections(Arc arc, Vector3[] positions)
        {
            for (int intersection_index = 0; intersection_index < positions.Length; ++intersection_index)
            {
                add_intersection(arc, new NormalizedCartesianCoordinates(positions[intersection_index]));
            }
        }

        private static void add_intersection(Arc arc, NormalizedCartesianCoordinates position)
        {
            if (position.data.y <= 0) // FIXME:
            {
                if (!discontinuities.ContainsKey(arc))
                {
                    discontinuities.Add(arc, new List<Discontinuity>());
                }

                discontinuities[arc].Add(new Discontinuity(arc, position));
            }
        }

        /// <summary>
        /// Mutator - find all intersections along x=0 or z=0 arcs in southern hemisphere.
        /// </summary>
        /// <param name="block">The block (set of arcs) to be inspected.</param>
        private static void find_discontinuities(PlanetariaCollider collider)
        {
            foreach (Arc arc in collider.shape.arcs)
            {
                for (int dimension = 0; dimension < 2; ++dimension) // Intersect already gets quadrants 3-4 by proxy
                {
                    float angle = (Mathf.PI/2)*dimension;
                    Vector3 begin = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                    Vector3 end = Vector3.down;
                    Vector3[] intersections = PlanetariaIntersection.arc_path_intersections(arc, begin, end, 0);
                    add_intersections(arc, intersections);
                }
            }
        }

        public static void prepare(PlanetariaCollider collider)
        {
            discontinuities = new Dictionary<Arc, List<Discontinuity>>();
            find_discontinuities(collider);
            sort_discontinuities();
            collider_variable = collider;
        }

        /// <summary>
        /// Mutator - Sort the Discontinuity lists in non-decreasing order (with respect to angle)
        /// </summary>
        private static void sort_discontinuities()
        {
            foreach (KeyValuePair<Arc, List<Discontinuity>> discontinuity in discontinuities)
            {
                discontinuity.Value.Sort((left_hand_side, right_hand_side) => left_hand_side.angle.CompareTo(right_hand_side.angle));
            }
        }

        public static IEnumerable<ArcIterator> arc_iterator()
        {
            foreach (Arc arc in collider_variable.shape.arcs)
            { 
                float begin_angle = -arc.angle()/2; // Start at the beginning of the arc (i.e. -arc.angle()/2)

                if (discontinuities.ContainsKey(arc))
                {
                    for (int list_index = 0; list_index < discontinuities[arc].Count; ++list_index)
                    {
                        float end_angle = discontinuities[arc][list_index].angle;
                        yield return new ArcIterator(arc, begin_angle, end_angle);
                        begin_angle = end_angle;
                    }
                }
            
                yield return new ArcIterator(arc, begin_angle, +arc.angle()/2); // The last segment goes until the end (i.e. +arc.angle()/2)
            }
        }

        private static PlanetariaCollider collider_variable;
        private static Dictionary<Arc, List<Discontinuity>> discontinuities;
    }
}

#endif

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.