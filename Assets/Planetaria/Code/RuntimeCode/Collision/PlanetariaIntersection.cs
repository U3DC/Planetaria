﻿using System;
using System.Linq; // TODO: Linq affects garbage collector (which affects virtual reality), also collisions are crazy inefficient
using UnityEngine;

namespace Planetaria
{
    public static class PlanetariaIntersection
    {
        /// <summary>
        /// Inspector - determines the intersection point of two neighboring arcs.
        /// </summary>
        /// <param name="a">The left arc.</param>
        /// <param name="b">The right arc (neighboring left).</param>
        /// <param name="extrusion">The extrusion distance of both arcs</param>
        /// <returns></returns>
        public static optional<Vector3> arc_arc_intersection(Arc a, Arc b, float extrusion)
        {
            Vector3[] intersections = circle_circle_intersections(a.floor(extrusion).collider(), b.floor(extrusion).collider());
            intersections = nontrivial_arc_intersections(a, b, intersections);
            intersections = valid_arc_intersections(a, intersections, Quaternion.identity);
            intersections = valid_arc_intersections(b, intersections, Quaternion.identity);
            if (intersections.Length == 0)
            {
                return new optional<Vector3>();
            }
            return intersections[0];
        }

        public static Vector3[] arc_path_intersections(Arc arc, Vector3 begin, Vector3 end, float extrusion)
        {
            SphericalCap arc_circle = arc.floor(extrusion).collider();
            SphericalCap path_circle = SphericalCap.cap(Vector3.Cross(begin, end).normalized, 0);

            Vector3[] intersections = circle_circle_intersections(arc_circle, path_circle);
            return valid_arc_intersections(arc, intersections, Quaternion.identity);
        }

        public static optional<Vector3> arc_path_intersection(Arc arc, Vector3 begin, Vector3 end, float extrusion)
        {
            Vector3[] intersections = arc_path_intersections(arc, begin, end, extrusion);

            // Note: the collision should be the first in front, but as long as the velocity is capped this is not an issue.
            if (intersections.Length == 0)
            {
                return new optional<Vector3>();
            }

            Vector3 intersection = intersections.Aggregate(
                    (closest_intersection, next_intersection) =>
                    Vector3.Dot(closest_intersection, begin) > Vector3.Dot(next_intersection, begin) ?
                    closest_intersection : next_intersection);

            return intersection;
        }

        /// <summary>
        /// Inspector - Returns intersection information between two circles on a sphere.
        /// </summary>
        /// <param name="coordinate_a">A point on a unit sphere and its radius along the surface.</param>
        /// <param name="coordinate_b">A point on a unit sphere and its radius along the surface.</param>
        /// <returns>
        /// If there are zero or infinite solutions, returns an empty array;
        /// If there are one or two solutions, returns an array with two Cartesian coordinates.
        /// </returns>
        public static Vector3[] circle_circle_intersections(SphericalCap a, SphericalCap b) // https://gis.stackexchange.com/questions/48937/calculating-intersection-of-two-circles
        {
            float similarity = Vector3.Dot(a.normal, b.normal);

            if (Mathf.Abs(similarity) > 1f - Precision.tolerance) // ignore points that are 1) equal or 2) opposite (within an error margin) because they will have infinite solutions
            {
                return new Vector3[0];
            }

            float a_radius = Mathf.Abs(Mathf.Acos(a.offset)); // TODO: simplify, elegance, efficiency (this will be one of many bottlenecks)
            float b_radius = Mathf.Abs(Mathf.Acos(b.offset));

            float arc_distance = Mathf.Acos(similarity);
            float radii_sum = a_radius + b_radius;
            float radii_difference = Mathf.Abs(a_radius - b_radius);

            bool adjacent = arc_distance < radii_sum - Precision.tolerance;
            bool does_not_engulf = radii_difference + Precision.tolerance < arc_distance;
            bool intersects = does_not_engulf && adjacent;

            if (!intersects) // solution set will be null
            {
                return new Vector3[0];
            }

            // Instead of thinking of a circle on a globe, it's easier to think of the circle as the intersection of a sphere against the surrounding globe
            float distance_from_origin_a = Mathf.Cos(a_radius);
            float distance_from_origin_b = Mathf.Cos(b_radius);
        
            // The center should be distance_from_origin away iff similarity = 0, otherwise you have to push the center closer or further
            float center_fraction_a = (distance_from_origin_a - distance_from_origin_b * similarity) / (1 - similarity * similarity);
            float center_fraction_b = (distance_from_origin_b - distance_from_origin_a * similarity) / (1 - similarity * similarity);

            Vector3 intersection_center = center_fraction_a*a.normal + center_fraction_b*b.normal;

            Vector3 binormal = Vector3.Cross(a.normal, b.normal);

            float midpoint_distance = Mathf.Sqrt((1 - intersection_center.sqrMagnitude) / binormal.sqrMagnitude); //CONSIDER: rename?

            // Note: tangential circles (i.e. midpoint_distance = 0) return two intersections (i.e. a secant line)
            Vector3[] intersections = new Vector3[2];
            intersections[0] = intersection_center + midpoint_distance*binormal;
            intersections[1] = intersection_center - midpoint_distance*binormal;
            return intersections;
        }

        public static Vector3[] raycast_intersection(Arc raycast_arc, Arc geometry_arc, float raycast_angle, Quaternion orientation)
        {
            SphericalCap relative_geometry_circle = geometry_arc.floor().collider();
            if (orientation != Quaternion.identity)
            {
                Quaternion arc_to_world = orientation;
                Debug.DrawRay(Vector3.zero, relative_geometry_circle.normal, Color.green, 1f);
                relative_geometry_circle = SphericalCap.cap(arc_to_world * relative_geometry_circle.normal, relative_geometry_circle.offset);
                Debug.DrawRay(Vector3.zero, relative_geometry_circle.normal, Color.cyan, 1f);
            }
            Vector3[] intersections = circle_circle_intersections(raycast_arc.floor().collider(), relative_geometry_circle);
            intersections = valid_arc_intersections(raycast_arc, intersections, Quaternion.identity, raycast_angle);
            intersections = valid_arc_intersections(geometry_arc, intersections, orientation);
            return intersections;
        }

        public static Vector3[] nontrivial_arc_intersections(Arc arc_a, Arc arc_b, Vector3[] intersections)
        {
            Vector3[] results = new Vector3[0];
            for (int intersection_index = 0; intersection_index < intersections.Length; ++intersection_index)
            {
                Vector3 intersection_point = intersections[intersection_index];
                if (Vector3.Dot(arc_a.begin(), arc_b.end()) > 1 - Precision.threshold)
                {
                    if (Vector3.Dot(arc_a.begin(), intersection_point) > 1 - Precision.threshold)
                    {
                        continue;
                    }
                }

                if (Vector3.Dot(arc_a.end(), arc_b.begin()) > 1 - Precision.threshold)
                {
                    if (Vector3.Dot(arc_a.end(), intersection_point) > 1 - Precision.threshold)
                    {
                        continue;
                    }
                }

                Array.Resize(ref results, results.Length + 1);
                results[results.Length - 1] = intersections[intersection_index];
            }

            return results;
        }

        public static Vector3[] valid_arc_intersections(Arc arc, Vector3[] intersections, // TODO: research and development
                Quaternion orientation,
                optional<float> max_angle = new optional<float>())
        {
            if (!max_angle.exists)
            {
                max_angle = arc.angle();
            }

            Vector3[] results = new Vector3[0];
            for (int intersection_index = 0; intersection_index < intersections.Length; ++intersection_index)
            {
                Vector3 intersection_point = intersections[intersection_index];
                if (orientation != Quaternion.identity)
                {
                    Quaternion arc_to_world = orientation;
                    Quaternion world_to_arc = Quaternion.Inverse(arc_to_world);
                    intersection_point = world_to_arc * intersection_point;
                }

                Vector3 arc_left = arc.position(-max_angle.data/2);
                Vector3 arc_center = arc.position(0);
                Vector3 arc_right = arc.position(+max_angle.data/2);
                Vector3 boundary_midpoint = (arc_left + arc_right)/2;

                Plane arc_validator = new Plane(arc_center, boundary_midpoint);
                if (arc_validator.GetSide(intersection_point))
                {
                    Array.Resize(ref results, results.Length + 1);
                    results[results.Length-1] = intersections[intersection_index];
                }
            }
                
            return results;
        }
    }
}

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