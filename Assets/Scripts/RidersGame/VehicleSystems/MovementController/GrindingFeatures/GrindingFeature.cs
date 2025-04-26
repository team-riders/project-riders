using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindingFeature : IDataPipelineStep<Blackboard>
    {
        public float feetOffsetY = 0.9f;

        public Blackboard ProcessData(Blackboard data)
        {
            Rigidbody rigidbody = data.GetValue<Rigidbody>("Rigidbody");
            SplineContainer pathContainer = data.GetValue<GrindPath>("CurrentPath").SplineContainer;
            float acceleration = data.GetValue<float>("FinalAcceleration");
            float currentProgress = data.GetValue<float>("CurrentProgress");

            Transform transform = rigidbody.transform;
            var spline = pathContainer.Spline;

            // Progression direction (might be set at the start of the grind)
            Vector3 pathDirection = spline.EvaluateTangent(currentProgress);
            Vector3 vehicleDirection = transform.forward;
            float dot = Vector3.Dot(pathDirection, vehicleDirection);
            int progressDirection = dot > 0f ? 1 : -1;

            // Find the direction that we need to move for this 

            // Need to make and scale velocity here now.
            float speed = acceleration * Time.fixedDeltaTime;

            // Convert that speed to a percentage of the path length
            float pathLength = spline.GetLength();
            float progressDelta = speed / pathLength;
            currentProgress += progressDelta * progressDirection;

            currentProgress = Mathf.Clamp01(currentProgress);

            // Get the new position and rotation
            float3 localPos = SplineUtility.EvaluatePosition(spline, currentProgress);
            float3 tangent = SplineUtility.EvaluateTangent(spline, currentProgress);

            Vector3 worldPos = pathContainer.transform.TransformPoint((Vector3)localPos);
            Vector3 forward = pathContainer.transform.TransformDirection((Vector3)tangent);
            Vector3 targetPos = worldPos + Vector3.up * feetOffsetY;

            Quaternion targetRot = Quaternion.LookRotation(forward);


            data.SetValue("CurrentProgress", currentProgress);
            rigidbody.position = targetPos;
            rigidbody.rotation = targetRot;

            return data;
        }
    }
}