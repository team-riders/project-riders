using System;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{

    public class JumpFeature : ConditionalDataPipelineStep<Blackboard>
    {
        const float JumpForce = 300f;
        const int JumpBoostHoldFrameThreshold = 180;
        const float JumpSpeedBoost = 0.5f;
        const float JumpBoostHoldMaxDuration = 120f;
        const float JumpChargeMinScale = 0.5f;
        const float JumpChargeMaxScale = 1.0f;

        public override Blackboard OnStep(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            Rigidbody rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");

            float GroundPercent = blackboard.GetValue<float>("GroundPercent");
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");

            bool WantsToJump = input.Jump;
            float WantsToHold = input.JumpHoldDuration;

            // We don't deal with any of this crap if we are not on the ground
            if (GroundPercent <= 0.0f) return blackboard;

            float JumpCharge = 0;

            if (WantsToHold > 0)
            {
                JumpCharge = Mathf.Clamp(WantsToHold / JumpBoostHoldMaxDuration, JumpChargeMinScale, JumpChargeMaxScale);
                if (WantsToHold > JumpBoostHoldFrameThreshold)
                {
                    maxSpeed *= JumpSpeedBoost;
                }
            }

            if (WantsToJump)
            {
                rigidbody.AddForce(Vector3.up * (JumpForce * JumpCharge), ForceMode.Impulse);
                blackboard.SetValue("MaxSpeed", maxSpeed);
                if (blackboard.ContainsKey("GrindPath"))
                {
                    blackboard.Remove("GrindPath");
                }
            }

            return blackboard;
        }

        public JumpFeature(Func<bool> condition = null) : base(condition)
        {
        }
    }
}