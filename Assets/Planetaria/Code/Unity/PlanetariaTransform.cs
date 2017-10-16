﻿using UnityEngine;

public class PlanetariaTransform
{
    public PlanetariaTransform(Transform internal_transform)
    {
        cartesian_transform = internal_transform;
    }

    public NormalizedSphericalCoordinates position
    {
        get
        {
            return position_variable;
        }

        set
        {
            dirty_position = true;
            previous_position_variable = position_variable;
            position_variable = value;
        }
    }

    public NormalizedSphericalCoordinates previous_position
    {
        get
        {
            return position_variable;
        }
    }

    public float rotation
    {
        get
        {
            return rotation_variable;
        }

        set
        {
            dirty_position = true;
            rotation_variable = value;
        }
    }

    public float scale
    {
        get
        {
            return scale_variable;
        }

        set
        {
            dirty_scale = true;
            scale_variable = value;
        }
    }

    public void move()
    {
        if (dirty_position)
        {
            cartesian_transform.rotation = Quaternion.AngleAxis(rotation_variable, position_variable.data);
            dirty_position = false;
        }

        if (dirty_scale)
        {
            cartesian_transform.localScale = Mathf.Sin(scale)*Vector3.one;
            dirty_scale = false;
        }
    }

    private Transform cartesian_transform;
    private NormalizedSphericalCoordinates position_variable;
    private float rotation_variable;
    private float scale_variable;

    private bool dirty_position;
    private bool dirty_scale;

    private NormalizedSphericalCoordinates previous_position_variable;
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