﻿using UnityEngine;

public class NormalizedSphericalCoordinates
{
	public Vector2 data
    {
        get { return data_; }
        set { data_ = value; Normalize(); }
    }

    public NormalizedSphericalCoordinates(float inclination, float azimuth)
    {
        data_ = new Vector2(inclination, azimuth);
        Normalize();
    }

    // implicit conversions: implement to_Cartesian and use it to make the other two functions!

    Vector2 data_;

    void Normalize() //TODO: Is this how phi/theta normalization works? (Is "normalization" even well-defined?)
    {
        if (Mathf.Abs(data_.x - Mathf.PI/2) > Mathf.PI/2)
        {
            data_.x = Mathf.PingPong(data_.x, 2*Mathf.PI); //TODO: test that 1) Vector2 is properly assigned 2) PingPong works for negative numbers
            if (data_.x > Mathf.PI)
            {
                data_.x -= Mathf.PI;
                data_.y += Mathf.PI; // going through a pole changes the azimuth
            }
        }
        if (Mathf.Abs(data_.y - Mathf.PI) > Mathf.PI || data_.y == 2*Mathf.PI)
        {
            data_.y = Mathf.PingPong(data_.y, 2*Mathf.PI);
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