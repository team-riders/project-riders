using System;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem.Features
{
    public class GrindJumpInputExitFeature : ConditionalDataPipelineStep<Blackboard>
    {
        const float JumpForce = 300f;
        const int JumpBoostHoldFrameThreshold = 180;
        const float JumpSpeedBoost = 0.5f;
        const float JumpBoostHoldMaxDuration = 120f;
        const float JumpChargeMinScale = 0.5f;
        const float JumpChargeMaxScale = 1.0f;

        public override Blackboard OnStep(Blackboard blackboard)
        {
            // Not implemented yet.
            // This is an alternative exit condition for the grind.
            // This will be a mixture of the JumpFeature and the GrindProgressCompleteExitFeature.
            // Please refer to the two.
            // The exit condition is if a valid jump input is provided
            // The exit values is a Rigidbody velocity + jump force going up.

            return blackboard;
        }

        public GrindJumpInputExitFeature(Func<bool> condition = null) : base(condition) { }
    }
}