using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class GrindPath : MonoBehaviour
{
    public bool loop = false;
    public bool reverse = false;
    public float speed = 7f; // m/s

    public SplineContainer SplineContainer => GetComponent<SplineContainer>();

    public float GetPercentagePerSecond()
    {
        var spline = SplineContainer.Spline;
        float length = spline.GetLength();
        return length > 0f ? speed / length : 0f;
    }
}
