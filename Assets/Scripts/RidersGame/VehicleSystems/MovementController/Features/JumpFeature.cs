using RidersRuntime.Input;
using System;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{

    public class JumpFeature : DataPipelineStep<Blackboard>
    {
        const float JumpForce = 600f;
        const int JumpBoostHoldFrameThreshold = 180;
        const float JumpSpeedBoost = 0.5f;
        const float JumpBoostHoldMaxDuration = 60f;
        const float JumpChargeMinScale = 0.5f;
        const float JumpChargeMaxScale = 1.0f;

        float localJumpCharge = 0f;

        public override Blackboard OnStep(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            Rigidbody rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");

            float GroundPercent = blackboard.GetValue<float>("GroundPercent");
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");

            bool WantsToJump = blackboard.GetValue<bool>("JumpIntention");
            float WantsToHold = input.JumpHoldDuration;

            // This feature won't actually be working for grinding so we can do ground checks here

            if (GroundPercent <= 0.0f)
            {
                localJumpCharge = 0f;
                return blackboard;
            }

            if (WantsToHold > 0)
            {
                localJumpCharge = Mathf.Clamp(WantsToHold / JumpBoostHoldMaxDuration, JumpChargeMinScale, JumpChargeMaxScale);
                if (WantsToHold > JumpBoostHoldFrameThreshold)
                {
                    maxSpeed *= JumpSpeedBoost;
                }
            }

            if (WantsToJump)
            {
                rigidbody.AddForce(Vector3.up * (JumpForce * localJumpCharge), ForceMode.Impulse);
                blackboard.SetValue("MaxSpeed", maxSpeed);
                if (blackboard.ContainsKey("GrindPath"))
                {
                    blackboard.Remove("GrindPath");
                }

                localJumpCharge = 0f;
            }

            blackboard.SetValue("JumpCharge", localJumpCharge);
            blackboard.SetValue("JumpIntention", false);
            blackboard.SetValue("JumpChargeMinSale", JumpChargeMinScale);
            return blackboard;
        }
    }
}