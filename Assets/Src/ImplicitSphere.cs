using UnityEngine;

public class ImplicitSphere : ImplicitSurface
{
    public float Radius;
    
    public override float ComputeFieldDistance(Vector3 point)
    {
        return ( point - transform.position ).magnitude;
    }

    public override SurfaceBounds GetSurfaceBounds()
    {
        Vector3 center          = transform.position;
        Vector3 radius_vector   = new Vector3(Radius, Radius, Radius);

        return new SurfaceBounds(
            center + radius_vector,
            center - radius_vector
        );
    }

    public override float GetIsoValue()
    {
        return Radius;
    }
};