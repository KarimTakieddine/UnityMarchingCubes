using UnityEngine;

public readonly struct SurfaceBounds
{
    public Vector3 Maximum { get; }
    public Vector3 Minimum { get; }

    public SurfaceBounds(
        Vector3 maximum,
        Vector3 minimum
    )
    {
        Maximum = maximum;
        Minimum = minimum;
    }
};

public abstract class ImplicitSurface : MonoBehaviour
{
    public abstract float GetIsoValue();

    public abstract float ComputeFieldDistance(Vector3 point);

    public abstract SurfaceBounds GetSurfaceBounds();
};