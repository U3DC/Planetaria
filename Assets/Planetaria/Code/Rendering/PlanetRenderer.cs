﻿using UnityEngine;

namespace Planetaria
{
    [ExecuteInEditMode]
    public class PlanetRenderer : PlanetariaRenderer // CONSIDER: rename SphereRenderer or SurfaceRenderer?
    {
        private void Awake()
        {
            set_transformation("PlanetRenderer");
            set_renderer();
            set_renderer_values();
            set_layer();
            internal_renderer.sharedMaterial = material;
            scalable = false;
        }

        protected sealed override void set_renderer()
        {
            internal_renderer = internal_transformation.GetOrAddComponent<MeshRenderer>();
            internal_mesh_filter = internal_transformation.GetOrAddComponent<MeshFilter>();
            internal_mesh_filter.sharedMesh = Octahedron.octahedron_mesh();
        }

        private MeshFilter internal_mesh_filter;
    }
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/