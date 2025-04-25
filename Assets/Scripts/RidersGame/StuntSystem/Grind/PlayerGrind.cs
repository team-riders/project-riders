using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

using RidersRuntime.Input;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(BaseVehicle))]
    [RequireComponent(typeof(GrindDetectorTrigger))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerGrind : MonoBehaviour
    {
        [Header("Grind Settings")]
        public float feetOffsetY = 0.9f;
        public float autoGrindRadius = 1.5f;
        public float rotationSpeed = 10f;
        public float minimumDownwardSpeed = 0.5f;

        [Header("Debug")]
        public bool showGizmo = true;
        private BaseVehicle baseVehicle;
        private Rigidbody rb;
        private PlayerInput playerInput;
        private GrindDetectorTrigger detector;

        private GrindPath currentPath;
        private float currentProgress;
        private float progressRate;
        private float cooldownTimer = 0f;
        private bool isGrinding = false;

        private void Awake()
        {
            baseVehicle = GetComponent<BaseVehicle>();
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();
            detector = GetComponent<GrindDetectorTrigger>();
        }

        private void Update()
        {
            if (cooldownTimer > 0f)
                cooldownTimer -= Time.deltaTime;

            if (isGrinding)
            {
                UpdateGrind();

                if (playerInput.GrabCurrentFrameInputs().Jump)
                {
                    Debug.Log("[PlayerGrind] Jump pressed. Exiting grind.");
                    StopGrinding();
                }
            }
            else if (baseVehicle.AirPercent > 0 && rb.linearVelocity.y < minimumDownwardSpeed)
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
            progressRate = Mathf.Max(path.GetPercentagePerSecond(rb.linearVelocity.magnitude), 0.0001f); // prevent 0-speed
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

            if (true)
            {
                transform.position = targetPos;
            }

            Quaternion targetRot = Quaternion.LookRotation(forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }

        private void StopGrinding()
        {
            if (!isGrinding) return;

            isGrinding = false;
            currentPath = null;
            currentProgress = 0f;
            cooldownTimer = 0.3f;

            // On a stop grind, we need to "kick" the player off
            // Apply a forced jump input? This should probably be done in the state machine to decouple

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
}