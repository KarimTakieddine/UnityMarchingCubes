using UnityEngine;

public class ImplicitSphere : ImplicitSurface
{
    public float Radius;
    
    public override float ComputeFieldDistance(Vector3 point)
    {
        return ( point - transform.position ).magnitude;
    }

    public override float GetIsoValue()
    {
        return Radius;
    }
};