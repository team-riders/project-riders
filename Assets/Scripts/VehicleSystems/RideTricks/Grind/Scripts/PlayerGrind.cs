using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(GrindDetectorTrigger))]
public class PlayerGrind : MonoBehaviour
{
    [Header("Grind Settings")]
    public float feetOffsetY = 0.9f;
    public float autoGrindRadius = 1.5f;
    public float grindSpeed = 7f;
    public float alignmentSpeed = 10f;

    [Header("Debug")]
    public bool showGizmo = true;

    private CharacterController controller;
    private GrindDetectorTrigger detector;

    private GrindPath currentPath;
    private float currentProgress;
    private float progressRate;
    private float cooldownTimer = 0f;
    private bool isGrinding = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        detector = GetComponent<GrindDetectorTrigger>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isGrinding)
        {
            UpdateGrind();

            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("[PlayerGrind] Jump pressed. Exiting grind.");
                StopGrinding();
            }
        }
        else if (!controller.isGrounded)
        {
            TryAutoGrind(autoGrindRadius);
        }
    }

    public bool IsGrinding() => isGrinding;

    public void TryAutoGrind(float radius)
    {
        if (cooldownTimer > 0f || isGrinding || detector == null)
            return;

        GrindPath nearest = detector.GetNearestPathWithinRadius(radius);
        if (nearest != null)
        {
            float progress = GetStartProgress(nearest);
            StartGrinding(nearest, progress);
        }
    }

    private float GetStartProgress(GrindPath path)
    {
        Vector3 localPos = path.splineTransform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(path.SplineContainer.Spline, (float3)localPos, out _, out float progress);
        return Mathf.Clamp01(progress);
    }

    private void StartGrinding(GrindPath path, float startProgress)
    {
        if (path == null || path.SplineContainer?.Spline == null || path.splineTransform == null)
        {
            Debug.LogWarning("[PlayerGrind] Invalid grind path.");
            return;
        }

        currentPath = path;
        currentProgress = startProgress;
        progressRate = Mathf.Max(path.GetPercentagePerSecond(), 0.0001f); // prevent 0-speed
        isGrinding = true;

        Debug.Log($"[PlayerGrind] Started grinding on '{path.name}' at progress {currentProgress:F4}");
    }

    private void UpdateGrind()
    {
        if (currentPath?.SplineContainer?.Spline == null)
        {
            StopGrinding();
            return;
        }

        currentProgress += progressRate * Time.deltaTime;

        if (!currentPath.loop && (currentProgress > 1f || currentProgress < 0f))
        {
            StopGrinding();
            return;
        }

        currentProgress %= 1f;

        var spline = currentPath.SplineContainer.Spline;
        float3 localPos = SplineUtility.EvaluatePosition(spline, currentProgress);
        float3 tangent = SplineUtility.EvaluateTangent(spline, currentProgress);

        Vector3 worldPos = currentPath.splineTransform.TransformPoint((Vector3)localPos);
        Vector3 forward = currentPath.splineTransform.TransformDirection((Vector3)tangent);
        Vector3 targetPos = worldPos + Vector3.up * feetOffsetY;

        if (controller.enabled)
        {
            controller.Move(targetPos - transform.position);
        }

        Quaternion targetRot = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * alignmentSpeed);
    }

    private void StopGrinding()
    {
        if (!isGrinding) return;

        isGrinding = false;
        currentPath = null;
        currentProgress = 0f;
        cooldownTimer = 0.3f;

        Debug.Log("[PlayerGrind] Grind ended.");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, autoGrindRadius);
    }
#endif
}
