﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Planetaria
{
    [CustomEditor(typeof(Field))]
    public class FieldEditor : Editor
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void draw_block_gizmos(Field self, GizmoType gizmo_type)
        {
            if (LevelCreatorEditor.debug_rendering)
            {
                foreach (Arc arc in self.iterator())
                {
                    if (!self.is_dynamic)
                    {
                        ArcEditor.draw_arc(arc);
                    }
                    else
                    {
                        ArcEditor.draw_arc(arc, self.GetComponent<Transform>());
                    }
                }
            }
        }
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