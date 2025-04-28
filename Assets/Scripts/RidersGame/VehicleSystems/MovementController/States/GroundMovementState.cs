using RidersRuntime.Data;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class GroundMovementState : MovementStateAlt
    {
        public GroundMovementState() : base("Ground Movement State")
        {
            _featureQueue = new(new() {
                new AccelerationFeature(),
                new SteerTurnFeature(),
                new JumpFeature(IsGrounded),
                new StopAccelerationPastMaxSpeed(),
                new ScaleToFixedDeltaTime(),
                new ClampToMaxSpeedOnGroundFeature(),
                new CoastingFeature(),
                // // new DriftFeature()
                new RotateToForwardsFeature(),
                new KeepUprightFeature()
            });
        }

        bool IsGrounded()
        {
            if (_refBlackboard == null) return false;
            if (_refBlackboard.ContainsKey("GroundPercent"))
            {
                return _refBlackboard.GetValue<float>("GroundPercent") > 0.0f;
            }
            return false;
        }
    }

    // ? DOWN HERE ARE SOME LAZY METHODS I PUT IN

    public class StopAccelerationPastMaxSpeed : DataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard blackboard)
        {
            float currentSpeed = blackboard.GetValue<float>("CurrentSpeed");
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");
            Vector3 movement = blackboard.GetValue<Vector3>("MovementVector");
            bool isBraking = blackboard.GetValue<bool>("IsBraking");

            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;
            if (wasOverMaxSpeed && !isBraking)
                movement *= 0.0f;

            blackboard.SetValue("MovementVector", movement);
            blackboard.SetValue("WasOverMaxSpeed", wasOverMaxSpeed);

            return blackboard;
        }
    }

    public class ScaleToFixedDeltaTime : DataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard blackboard)
        {
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            Vector3 movement = blackboard.GetValue<Vector3>("MovementVector");

            Vector3 newVelocity = rb.linearVelocity + movement * Time.fixedDeltaTime;
            newVelocity.y = rb.linearVelocity.y;

            rb.linearVelocity = newVelocity;

            return blackboard;
        }
    }

    public class ClampToMaxSpeedOnGroundFeature : DataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard blackboard)
        {
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");
            float GroundPercent = blackboard.GetValue<float>("GroundPercent");
            bool wasOverMaxSpeed = blackboard.GetValue<bool>("WasOverMaxSpeed");

            //  clamp max speed if we are on ground
            if (GroundPercent > 0.0f && !wasOverMaxSpeed)
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
            }

            return blackboard;
        }
    }

    public class CoastingFeature : DataPipelineStep<Blackboard>
    {
        public const float k_NullInput = 0.01f;
        public override Blackboard OnStep(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            VehicleStats stats = blackboard.GetValue<VehicleStats>("VehicleStats");
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            float GroundPercent = blackboard.GetValue<float>("GroundPercent");

            float accelInput = input.Accelerate - input.Brake;

            if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
            {
                float coastingDelta = stats.CoastingDrag * Time.fixedDeltaTime;
                rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, new Vector3(0, rb.linearVelocity.y, 0), coastingDelta);
            }

            return blackboard;
        }
    }
}