using UnityEngine;

public abstract class ImplicitSurface : MonoBehaviour
{
    public abstract float GetIsoValue();

    public abstract float ComputeFieldDistance(Vector3 point);
};