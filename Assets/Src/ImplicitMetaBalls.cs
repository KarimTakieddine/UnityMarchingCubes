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

﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ImplicitMetaBalls : ImplicitSurface
{
    public List<MetaBall> MetaBalls;
    public float GooFactor;
    public float Threshold;

    public override float ComputeFieldDistance(Vector3 point)
    {
        int meta_ball_count = MetaBalls.Count;

        if ( meta_ball_count == 0 )
        {
            return Threshold;
        }

        float field_distance = 0.0f;

        for (int i = 0; i < meta_ball_count; ++i)
        {
            MetaBall meta_ball = MetaBalls[i];

            field_distance += meta_ball.Radius / Mathf.Pow( ( point - meta_ball.transform.position).magnitude, GooFactor );
        }

        return field_distance;
    }

    public override SurfaceBounds GetSurfaceBounds()
    {
        List<float> max_x_positions = new List<float>();
        List<float> min_x_positions = new List<float>();

        List<float> max_y_positions = new List<float>();
        List<float> min_y_positions = new List<float>();

        List<float> max_z_positions = new List<float>();
        List<float> min_z_positions = new List<float>();

        for (int i = 0; i < MetaBalls.Count; ++i)
        {
            MetaBall meta_ball = MetaBalls[i];

            Vector3 center  = meta_ball.transform.position;
            float radius    = meta_ball.Radius;

            float x_position = center.x;
            max_x_positions.Add(x_position + radius);
            min_x_positions.Add(x_position - radius);

            float y_position = center.y;
            max_y_positions.Add(y_position + radius);
            min_y_positions.Add(y_position - radius);

            float z_position = center.z;
            max_z_positions.Add(z_position + radius);
            min_z_positions.Add(z_position - radius);
        }

        return new SurfaceBounds(
            new Vector3(
                max_x_positions.Max(),
                max_y_positions.Max(),
                max_z_positions.Max()
            ),
            new Vector3(
                min_x_positions.Min(),
                min_y_positions.Min(),
                min_z_positions.Min()
            )
        );
    }

    public override float GetIsoValue()
    {
        return Threshold;
    }
};