using UnityEngine;

public class ImplicitPlane : ImplicitSurface
{
    public override float ComputeFieldDistance(Vector3 point)
    {
        Quaternion local_rotation = transform.rotation;

        Vector3 up_vector = Vector3.Cross(
            local_rotation * Vector3.right,
            local_rotation * Vector3.forward
        );

        return Vector3.Dot(point - transform.position, up_vector);
    }

    public override float GetIsoValue()
    {
        return 0.0f;
    }
};