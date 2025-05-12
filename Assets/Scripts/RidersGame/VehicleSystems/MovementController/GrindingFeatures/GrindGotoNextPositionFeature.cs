using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindingFeature : DataPipelineStep<Blackboard>
    {
        public float feetOffsetY = 0.2f;

        public override Blackboard OnStep(Blackboard data)
        {
            Rigidbody rigidbody = data.GetValue<Rigidbody>("Rigidbody");
            SplineContainer pathContainer = data.GetValue<GrindPath>("CurrentPath").SplineContainer;
            float currentProgress = data.GetValue<float>("CurrentProgress");
            int progressDirection = data.GetValue<int>("Direction");
            float speed = data.GetValue<float>("CurrentSpeed");

            var spline = pathContainer.Spline;

            float totalLength = spline.GetLength();
            float rateOfChange = speed * Time.fixedDeltaTime / totalLength;
            currentProgress += rateOfChange * progressDirection;
            currentProgress = Mathf.Clamp01(currentProgress);

            spline.Evaluate(currentProgress, out float3 position, out float3 tangent, out float3 normal);

            Vector3 worldPos = pathContainer.transform.TransformPoint((Vector3)position);
            Vector3 forward = pathContainer.transform.TransformDirection((Vector3)tangent) * progressDirection;
            Vector3 up = pathContainer.transform.TransformDirection((Vector3)normal);

            rigidbody.MovePosition(worldPos + up * feetOffsetY); // MovePosition is used to ensure the rigidbody is moved correctly in the physics simulation

            Quaternion targetRotation = Quaternion.LookRotation(forward, up);

            rigidbody.rotation = targetRotation;

            data.SetValue("CurrentProgress", currentProgress);
            return data;
        }
    }
}