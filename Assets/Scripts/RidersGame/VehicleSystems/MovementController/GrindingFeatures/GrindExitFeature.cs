using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindExitFeature : ConditionalDataPipelineStep<Blackboard>
    {
        const float KickoffSpeed = 5;
        const float HackExitSpeedNerfCoeff = 0.6f;

        public override bool IsConditionMet(Blackboard data)
        {
            // Check if the player is in the air and not on a grind path
            if (!data.ContainsKey("CurrentProgress"))
                return false;


            float progress = data.GetValue<float>("CurrentProgress");

            bool inProgress = progress > 0.0f && progress < 1.0f;

            return !inProgress;
        }

        public override Blackboard OnStep(Blackboard blackboard)
        {
            Rigidbody rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");
            GrindPath grindPath = blackboard.GetValue<GrindPath>("CurrentPath");
            float speed = blackboard.GetValue<float>("CurrentSpeed");
            float progressDirection = blackboard.GetValue<int>("Direction");

            Spline path = grindPath.SplineContainer.Spline;

            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            // Get the tangent and normal at the current progress
            path.Evaluate(blackboard.GetValue<float>("CurrentProgress"), out float3 position, out float3 tangent, out float3 normal);

            speed = Mathf.Max(speed, KickoffSpeed);

            // Apply forward velocity based on tangent direction
            Vector3 forward = grindPath.SplineContainer.transform.TransformDirection((Vector3)tangent) * progressDirection;
            Vector3 forwardExit = forward * speed * HackExitSpeedNerfCoeff;

            // Set the new velocity
            Vector3 newVelocity = forwardExit;
            rigidbody.linearVelocity = newVelocity;

            return blackboard;
        }
    }
}