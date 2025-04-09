using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class PlayerGrind : MonoBehaviour
{
    private GrindPath currentPath;
    private float currentProgress = 0f;
    private Vector3 rootPosition;
    private bool isGrinding = false;
    private float progressPerSecond = 0f;
    private bool reverse = false;

    void Update()
    {
        if (!isGrinding || currentPath == null) return;

        // Handle jump-off
        if (Input.GetButtonDown("Jump"))
        {
            EndGrind();
            return;
        }

        var spline = currentPath.SplineContainer.Spline;
        float delta = progressPerSecond * Time.deltaTime * (reverse ? -1f : 1f);
        currentProgress += delta;

        if (!currentPath.loop && (currentProgress >= 1f || currentProgress <= 0f))
        {
            EndGrind();
            return;
        }

        currentProgress = Mathf.Repeat(currentProgress, 1f);

        SplineUtility.Evaluate(spline, currentProgress, out float3 pos, out float3 tan, out float3 up);
        transform.SetPositionAndRotation(rootPosition + (Vector3)pos, quaternion.LookRotation(tan, up));
    }

    public void StartGrind(GrindPath path, float startProgress = 0f)
    {
        currentPath = path;
        rootPosition = path.transform.position;
        currentProgress = Mathf.Clamp01(startProgress);
        progressPerSecond = currentPath.GetPercentagePerSecond();
        reverse = currentPath.reverse;
        isGrinding = true;

        // Snap to start
        var spline = path.SplineContainer.Spline;
        SplineUtility.Evaluate(spline, currentProgress, out float3 pos, out float3 tan, out float3 up);
        transform.SetPositionAndRotation(rootPosition + (Vector3)pos, quaternion.LookRotation(tan, up));
    }

    public void EndGrind()
    {
        isGrinding = false;
        currentPath = null;
    }

    public bool IsGrinding() => isGrinding;
}
