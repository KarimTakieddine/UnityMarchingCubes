using UnityEngine;

public class ImplicitPlane : ImplicitSurface
{
    public float Width, Height, Depth;

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
        return new SurfaceBounds(
            new Vector3(Width, Height, Depth),
            Vector3.zero
        );
    }

    public override float GetIsoValue()
    {
        return 0.0f;
    }
};