﻿using UnityEngine;

namespace Planetaria
{
    public sealed class AreaRenderer : PlanetariaRenderer
    {
        // Properties (Public)

        public Color color
        {
            get
            {
                return sprite_renderer.color;
            }
            set
            {
                sprite_renderer.color = value;
            }
        }

        protected sealed override void set_renderer()
        {
            if (internal_renderer == null)
            {
                internal_renderer = Miscellaneous.GetOrAddComponent<SpriteRenderer>(internal_transform);
            }
            if (sprite_renderer == null)
            {
                sprite_renderer = Miscellaneous.GetOrAddComponent<SpriteRenderer>(internal_transform);
            }
            internal_renderer.sharedMaterial = material;

            SpriteRenderer renderer = Miscellaneous.GetOrAddComponent<SpriteRenderer>(internal_transform);
            renderer.sprite = sprite;
        }
        
        public Sprite sprite
        {
            get
            {
                return sprite_variable;
            }
            set
            {
                sprite_variable = value;
                sprite_renderer.sprite = sprite_variable;
            }
        }

        [SerializeField] private Sprite sprite_variable;
        [SerializeField] [HideInInspector] private SpriteRenderer sprite_renderer;
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