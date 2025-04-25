using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindBehaviour : MonoBehaviour
    {
        public float Speed = 7f; // m/s

        public float PercentagePerSecond = 0.1f; // Percentage of the path to move per second

        float currentProgress = 0f;
        public SplineContainer path;
        Vector3 rootPosition;
        Spline currentPath;

        void Start()
        {
            rootPosition = path.transform.position;
            BindToPath(path.Spline);
        }

        void BindToPath(Spline path)
        {
            SplineUtility.GetNearestPoint(path, transform.position, out float3 closestPoint, out currentProgress);
            currentPath = path;
            PercentagePerSecond = Speed / path.GetLength();
            transform.position = rootPosition + (Vector3)closestPoint;
        }

        void Update()
        {
            if (currentPath != null)
            {
                if (currentProgress >= 1f) currentPath = null;

                SplineUtility.Evaluate(currentPath, currentProgress + PercentagePerSecond * Time.deltaTime, out float3 position, out float3 tangent, out float3 upVector);

                transform.SetPositionAndRotation(rootPosition + (Vector3)position, quaternion.LookRotation(tangent, upVector));
                currentProgress += PercentagePerSecond * Time.deltaTime;
            }
        }
    }
}