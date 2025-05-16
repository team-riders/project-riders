using System;
using RidersRuntime.Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindJumpInputExitFeature : DataPipelineStep<Blackboard>
    {
        const float JumpForce = 300f;
        const int JumpBoostHoldFrameThreshold = 180;
        const float JumpSpeedBoost = 0.5f;
        const float JumpBoostHoldMaxDuration = 120f;
        const float JumpChargeMinScale = 0.5f;
        const float JumpChargeMaxScale = 1.0f;

        const float KickoffSpeed = 5;
        const float HackExitSpeedNerfCoeff = 1.2f;

        float localJumpCharge = 0f;

        public override Blackboard OnStep(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            Rigidbody rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");
            GrindPath grindPath = blackboard.GetValue<GrindPath>("CurrentPath");

            Spline path = grindPath.SplineContainer.Spline;

            float speed = blackboard.GetValue<float>("CurrentSpeed");
            float progressDirection = blackboard.GetValue<int>("Direction");
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");

            bool WantsToJump = blackboard.GetValue<bool>("JumpIntention");
            float WantsToHold = input.JumpHoldDuration;

            // Debug.Log("WantsToHold: " + WantsToHold);
            // Debug.Log("WantsToJump: " + WantsToJump);

            if (WantsToHold > 0)
            {
                localJumpCharge = Mathf.Clamp(WantsToHold / JumpBoostHoldMaxDuration, JumpChargeMinScale, JumpChargeMaxScale);
            }

            if (WantsToJump)
            {
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;

                float3 tangent = path.EvaluateTangent(blackboard.GetValue<float>("CurrentProgress"));

                Vector3 forward = grindPath.SplineContainer.transform.TransformDirection((Vector3)tangent) * progressDirection;
                forward.Normalize();

                Vector3 forwardExit = forward * speed * HackExitSpeedNerfCoeff;

                Vector3 newVelocity = forwardExit;
                rigidbody.linearVelocity = newVelocity;

                rigidbody.AddForce(Vector3.up * (JumpForce * localJumpCharge), ForceMode.Impulse);

                blackboard.Remove("CurrentPath");

                localJumpCharge = 0f;
            }

            blackboard.SetValue("JumpIntention", false);
            return blackboard;
        }
    }
}