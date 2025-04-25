using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(PlayerGrind))]
    public class GrindDetectorTrigger : MonoBehaviour
    {
        private PlayerGrind playerGrind;

        // Optional: legacy proximity trigger (not currently used)
        private readonly List<GrindPath> nearbyPaths = new();

        [Header("Debug Settings")]
        public bool showGizmo = true;
        public float debugRadius = 4.5f;
        public bool logDetection = false;

        void Awake()
        {
            playerGrind = GetComponent<PlayerGrind>();
        }

        /// <summary>
        /// Finds the closest GrindPath within a given radius using world-space proximity to the spline.
        /// </summary>
        public GrindPath GetNearestPathWithinRadius(float radius)
        {
            GrindPath[] allPaths = Object.FindObjectsByType<GrindPath>(FindObjectsSortMode.None);
            GrindPath closest = null;
            float closestDist = float.MaxValue;
            Vector3 playerPos = transform.position;

            if (logDetection)
                Debug.Log($"[GrindDetector] Scanning {allPaths.Length} GrindPaths...");

            foreach (var path in allPaths)
            {
                if (path == null || path.SplineContainer?.Spline == null || path.splineTransform == null)
                    continue;

                Vector3 localPos = path.splineTransform.InverseTransformPoint(playerPos);
                SplineUtility.GetNearestPoint(path.SplineContainer.Spline, (float3)localPos, out _, out float progress);
                float3 localSplinePos = SplineUtility.EvaluatePosition(path.SplineContainer.Spline, progress);
                Vector3 worldSplinePos = path.splineTransform.TransformPoint((Vector3)localSplinePos);

                float dist = Vector3.Distance(playerPos, worldSplinePos);

                if (dist <= radius && dist < closestDist)
                {
                    closest = path;
                    closestDist = dist;
                }

                if (logDetection)
                    Debug.Log($"[GrindDetector] → '{path.name}' | Distance: {dist:F2}");
            }

            if (logDetection)
            {
                string msg = closest != null ? $"Closest: {closest.name} ({closestDist:F2})" : "No valid path found";
                Debug.Log($"[GrindDetector] {msg}");
            }

            return closest;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmo) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up, debugRadius);
        }
#endif
    }

}