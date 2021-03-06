﻿using UnityEngine;

namespace Planetaria
{
    public sealed class VolumeRenderer : PlanetariaRenderer
    {
        // FIXME: For now, VolumeRenderer can draw the Mesh asset without alteration, but when zoom is implemented a flatten mesh shader will have to be implemented *with respect to (0,0,0), not the camera's position*

        protected sealed override void set_renderer()
        {
            if (internal_renderer == null)
            {
                internal_renderer = Miscellaneous.GetOrAddComponent<MeshRenderer>(internal_transform);
            }
            if (internal_mesh_filter == null)
            {
                internal_mesh_filter = Miscellaneous.GetOrAddComponent<MeshFilter>(internal_transform);
            }
            internal_renderer.sharedMaterial = material;
            internal_mesh_filter.sharedMesh = mesh;
        }

        [SerializeField] public Mesh mesh;
        [SerializeField] [HideInInspector] private MeshFilter internal_mesh_filter;
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