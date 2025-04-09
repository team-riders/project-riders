using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(CharacterController))]
public class PlayerGrind : MonoBehaviour
{
    private GrindPath currentPath;
    private float currentProgress = 0f;
    private bool isGrinding = false;
    private float progressPerSecond = 0f;
    private bool reverse = false;

    private CharacterController controller;

    [SerializeField]
    private float feetOffsetY = 0.9f; // Raise the player upward so feet meet the rail

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isGrinding || currentPath == null) return;

        // Handle jump-off input
        if (Input.GetButtonDown("Jump"))
        {
            EndGrind();
            return;
        }

        var spline = currentPath.SplineContainer.Spline;
        var splineTransform = currentPath.splineTransform;

        float delta = progressPerSecond * Time.deltaTime * (reverse ? -1f : 1f);
        currentProgress += delta;

        if (!currentPath.loop && (currentProgress >= 1f || currentProgress <= 0f))
        {
            EndGrind();
            return;
        }

        currentProgress = Mathf.Repeat(currentProgress, 1f);

        SplineUtility.Evaluate(spline, currentProgress, out float3 pos, out float3 tan, out float3 up);

        Vector3 worldPos = splineTransform.TransformPoint((Vector3)pos) + new Vector3(0, feetOffsetY, 0);
        Quaternion worldRot = quaternion.LookRotation(tan, up);

        transform.SetPositionAndRotation(worldPos, worldRot);
    }

    public void StartGrind(GrindPath path, float startProgress = 0f)
    {
        currentPath = path;

        if (path.splineTransform == null || path.SplineContainer == null)
        {
            Debug.LogError($"[PlayerGrind] ERROR: 'splineTransform' or 'SplineContainer' is not assigned in {path.gameObject.name}!");
            return;
        }

        var spline = path.SplineContainer.Spline;
        var splineTransform = path.splineTransform;

        // Sample player’s position (no offset)
        Vector3 localPos = splineTransform.InverseTransformPoint(transform.position);

        // Get the closest progress value on the spline
        SplineUtility.GetNearestPoint(spline, (float3)localPos, out float3 _, out currentProgress);
        Debug.Log($"[PlayerGrind] Nearest spline progress = {currentProgress:F4} from localPos {localPos}");

        progressPerSecond = currentPath.GetPercentagePerSecond();
        reverse = currentPath.reverse;
        isGrinding = true;

        controller.enabled = false;

        SplineUtility.Evaluate(spline, currentProgress, out float3 pos, out float3 tan, out float3 up);

        Vector3 worldPos = splineTransform.TransformPoint((Vector3)pos) + new Vector3(0, feetOffsetY, 0);
        Quaternion worldRot = quaternion.LookRotation(tan, up);

        transform.SetPositionAndRotation(worldPos, worldRot);
    }

    public void EndGrind()
    {
        isGrinding = false;
        currentPath = null;

        controller.enabled = true;
    }

    public bool IsGrinding() => isGrinding;
}
