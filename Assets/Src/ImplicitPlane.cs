/*-------------------------------------------------------------------------------------*\

##	Copyright © 2019 Karim Takieddine
##
##	Permission is hereby granted, free of charge, to any person obtaining a copy
##	of this software and associated documentation files (the "Software"), to deal
##	in the Software without restriction, including without limitation the rights
##	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
##	of the Software, and to permit persons to whom the Software is furnished to do so,
##	subject to the following conditions:
##
##	The above copyright notice and this permission notice shall be included in all
##	copies or substantial portions of the Software.
##
##	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
##	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
##	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
##	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
##	CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
##	OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*-------------------------------------------------------------------------------------*/

using UnityEngine;
using System.Linq;

public class ImplicitPlane : ImplicitSurface
{
    public float Width, Depth;

    public Vector3 GetUpVector()
    {
        Quaternion local_rotation = transform.localRotation;

        return Vector3.Cross(
            local_rotation * Vector3.right,
            local_rotation * Vector3.forward
        ).normalized;
    }

    public override float ComputeFieldDistance(Vector3 point)
    {
        return Vector3.Dot(
            point - transform.position,
            GetUpVector()
        );
    }

    public override SurfaceBounds GetSurfaceBounds()
    {
        Quaternion local_rotation   = transform.localRotation;

        Vector3 position            = new Vector3(0.0f, transform.position.y, 0.0f);
        Vector3 right               = position + ( local_rotation * Vector3.right ).normalized * Width;
        Vector3 forward             = position + ( local_rotation * Vector3.forward ).normalized * Depth;
        Vector3 forward_right       = position + ( local_rotation * new Vector3(1.0f, 0.0f, 1.0f) ).normalized *
                                                 new Vector3(Width, Depth).magnitude;

        float[] y_magnitudes = new float[4]
        {
            position.y,
            right.y,
            forward.y,
            forward_right.y
        };

        float max_y_magnitude   = y_magnitudes.Max();
        float min_y_magnitude  = y_magnitudes.Min();

        return new SurfaceBounds(
            new Vector3(Width, max_y_magnitude, Depth),
            new Vector3(0, min_y_magnitude, 0)
        );
    }

    public override float GetIsoValue()
    {
        return 0.0f;
    }
};