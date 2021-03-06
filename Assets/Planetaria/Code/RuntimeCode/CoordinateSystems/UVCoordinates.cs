﻿using System;
using UnityEngine;

namespace Planetaria
{
    /// <summary>
	///
    /// </summary>
    [Serializable]
	public struct UVCoordinates
	{
		// Properties (Public)

        public Vector2 data
        {
            get { return data_variable; }
        }
		
		// Methods (Public)

        /// <summary>
        /// Constructor - Stores the UV coordinates in a wrapper class.
        /// </summary>
        /// <param name="u">The u coordinate in UV space. Range: [0,1]</param>
        /// <param name="v">The v coordinate in UV space. Range: [0,1]</param>
        public UVCoordinates(float u, float v)
        {
            data_variable = new Vector2(u, v);
            normalize();
        }

		// Methods (non-Public)

		/// <summary>
        /// Mutator - Wrap UV coordinates so that neither value is outside of [0,1].
        /// </summary>
        private void normalize()
        {
            if (data_variable.x < 0 || data_variable.x > 1)
            {
                data_variable.x = PlanetariaMath.modolo_using_euclidean_division(data_variable.x, 1); // TODO: does this work?
            }
            else if (data_variable.x == 0)
            {
                data_variable.x = Precision.just_above_zero;
            }
            else if (data_variable.x == 1)
            {
                data_variable.x = Precision.just_below_one;
            }
            if (data_variable.y < 0 || data_variable.y > 1)
            {
                data_variable.y = PlanetariaMath.modolo_using_euclidean_division(data_variable.y, 1);
            }
            else if (data_variable.y == 0)
            {
                data_variable.y = Precision.just_above_zero;
            }
            else if (data_variable.y == 1)
            {
                data_variable.y = Precision.just_below_one;
            }
        }

		// Variables (non-Public)
		
        [SerializeField] private Vector2 data_variable; // FIXME: composition using "UVCoordinates"
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