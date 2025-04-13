using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class GrindPath : MonoBehaviour
{
    [Header("Grind Settings")]
    public bool loop = false;
    public bool reverse = false;
    public float speed = 7f;

    [Header("References")]
    public Transform splineTransform;
    public SplineContainer SplineContainer;

    /// <summary>
    /// Returns the progress rate (percentage per second) based on spline length and grind speed.
    /// </summary>
    public float GetPercentagePerSecond()
    {
        if (SplineContainer == null || SplineContainer.Spline == null)
        {
            Debug.LogError($"[GrindPath] Missing SplineContainer or Spline on '{gameObject.name}'");
            return 0f;
        }

        float length = SplineContainer.Spline.GetLength();
        if (length <= 0f)
        {
            Debug.LogWarning($"[GrindPath] Spline length is 0 on '{gameObject.name}'");
            return 0f;
        }

        float rate = speed / length;
        Debug.Log($"[GrindPath] '{gameObject.name}' | Speed: {speed} | Length: {length:F2} | Rate: {rate:F4}");
        return rate;
    }

#if UNITY_EDITOR
    [Header("Debug Gizmo")]
    public bool showGizmo = true;

    private void OnDrawGizmos()
    {
        if (!showGizmo || SplineContainer?.Spline == null) return;

        Gizmos.color = Color.green;
        const int resolution = 25;

        for (int i = 0; i < resolution; i++)
        {
            float t1 = i / (float)resolution;
            float t2 = (i + 1) / (float)resolution;

            var p1 = SplineContainer.Spline.EvaluatePosition(t1);
            var p2 = SplineContainer.Spline.EvaluatePosition(t2);

            Vector3 wp1 = splineTransform ? splineTransform.TransformPoint((Vector3)p1) : (Vector3)p1;
            Vector3 wp2 = splineTransform ? splineTransform.TransformPoint((Vector3)p2) : (Vector3)p2;

            Gizmos.DrawLine(wp1, wp2);
        }
    }
#endif
}
