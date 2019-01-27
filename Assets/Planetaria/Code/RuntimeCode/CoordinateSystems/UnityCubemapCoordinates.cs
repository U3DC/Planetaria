﻿using System;
using UnityEngine;

namespace Planetaria
{
    [Serializable]
    public struct UnityCubemapCoordinates
    {
        /// <summary>
        /// Constructor - Stores the Unity cube's UV coordinates in a Skybox-like wrapper class.
        /// </summary>
        /// <param name="u">The u coordinate in UV space for the Unity cube. Range: [0,1]</param>
        /// <param name="v">The v coordinate in UV space for the Unity cube. Range: [0,1]</param>
        /// <param name="face_index">[0,6) array index; 0=right, 1=left, 2=up, 3=down, 4=front, 5=back</param>
        public UnityCubemapCoordinates(float u, float v, int face_index)
        {
            uv_variable = new Vector2(u, v);
            face_index_variable = face_index;
            normalize();
        }

        public int texture_index
        {
            get { return face_index_variable; }
        }

        public Vector2 uv
        {
            get { return uv_variable; }
        }

        /// <summary>
        /// Inspector - Converts Unity cube's UV coordinates into normalized cartesian coordinates.
        /// </summary>
        /// <param name="skybox">The coordinates in the Unity cube / skybox UV space that will be converted.</param>
        /// <returns>The normalized cartesian coordinates.</returns>
        public static implicit operator NormalizedCartesianCoordinates(UnityCubemapCoordinates skybox)
        {
            return (NormalizedCubeCoordinates)skybox; // implicit chains of length three won't automatically work so convert OctahedralUVCoordinates -> NormalizedOctahedralCoordinates -> NormalizedCartesianCoordinates
        }

        /// <summary>
        /// Inspector - Converts Unity cube's UV coordinates into normalized cube coordinates.
        /// </summary>
        /// <param name="skybox">The coordinates in Unity cubemap / skybox UV space that will be converted.</param>
        /// <returns>The normalized cube coordinates. (At least one of x,y,and z will be magnitude 1.)</returns>
        public static implicit operator NormalizedCubeCoordinates(UnityCubemapCoordinates skybox) // FIXME: CONSIDER: I don't like the Unity internal format for Cubemap, or that this is heavily tied to that format.
        {
            float x = 2*skybox.uv_variable.x - 1;
            float y = 2*skybox.uv_variable.y - 1;
            switch (skybox.face_index_variable)
            {
                case 0:
                    return new NormalizedCubeCoordinates(new Vector3(+1, -y, -x));
                case 1:
                    return new NormalizedCubeCoordinates(new Vector3(-1, -y, x));
                case 2:
                    return new NormalizedCubeCoordinates(new Vector3(x, +1, y));
                case 3:
                    return new NormalizedCubeCoordinates(new Vector3(x, -1, -y));
                case 4:
                    return new NormalizedCubeCoordinates(new Vector3(x, -y, +1));
                case 5: default:
                    return new NormalizedCubeCoordinates(new Vector3(-x, -y, -1));
            }
        }

        public static int face(Vector3 cartesian)
        {
            float x_magnitude = Mathf.Abs(cartesian.x);
            float y_magnitude = Mathf.Abs(cartesian.y);
            float z_magnitude = Mathf.Abs(cartesian.z);

            if (x_magnitude >= Mathf.Max(y_magnitude, z_magnitude))
            {
                return Mathf.Sign(cartesian.x) == -1 ? 1 : 0;
            }
            else if (y_magnitude >= z_magnitude)
            {
                return Mathf.Sign(cartesian.y) == -1 ? 3 : 2;
            }
            return Mathf.Sign(cartesian.z) == -1 ? 5 : 4;
        }

        /// <summary>
        /// Mutator - Wrap UV coordinates so that neither uv coordinate value is outside of [0,1] and the face index is [0,6)
        /// </summary>
        private void normalize()
        {
            if (1 < uv_variable.x || uv_variable.x < 0) // modulo goes from [0,1) and we want [0,1]
            {
                uv_variable.x = PlanetariaMath.modolo_using_euclidean_division(uv_variable.x, 1);
            }
            if (1 < uv_variable.y || uv_variable.y < 0) // modulo goes from [0,1) and we want [0,1]
            {
                uv_variable.y = PlanetariaMath.modolo_using_euclidean_division(uv_variable.y, 1);
            }
            if (6 <= face_index_variable || face_index_variable < 0)
            {
                face_index_variable = (int)PlanetariaMath.modolo_using_euclidean_division(face_index_variable, 6); // HACK: CONSIDER: would anyone ever enter something like 2^31 in here?
            }
        }

        [SerializeField] private int face_index_variable;
        [SerializeField] private Vector2 uv_variable;
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