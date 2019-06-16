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