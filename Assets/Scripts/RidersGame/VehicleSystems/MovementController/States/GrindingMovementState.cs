using RidersRuntime.VehicleSystem;
using RidersRuntime.Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RidersRuntime.VehicleSystem
{
    public class GrindingMovementState : MovementStateAlt
    {
        public float minimumDownwardSpeed = 0.5f;
        public const float GrindCooldownDuration = 3f;
        public bool CanEnterGrind() => _grindCooldownTimer <= 0f && timerReady;
        public bool IsGrinding => !isComplete && !!currentPath;

        bool timerReady = true;

        private float _grindCooldownTimer = 0f;
        private bool isComplete = true;
        private GrindPath currentPath;

        public void DoCooldown()
        {
            if (_grindCooldownTimer > 0)
            {
                _grindCooldownTimer -= Time.fixedDeltaTime;
            }

            if (_grindCooldownTimer < 0)
            {
                _grindCooldownTimer = 0;
                timerReady = true;
            }
        }

        public GrindingMovementState() : base("Grinding Movement State")
        {
            _featureQueue = new(new()
            {
                new GrindEnterFeature(),
                new AccelerationNoRBFeature(),
                // To fix the momentum bug, we need to do the following sequence
                new NoRBStopAccelerationPastMaxSpeed(),
                new NoRBFakeGravityAccelerationFeature(),
                new NoRBMinimumSpeedFeature(),
                new NoRBScaleToFixedDeltaTime(),
                new GrindingFeature(),
                new GrindExitFeature(),
                new GrindJumpInputExitFeature(),
            });
        }

        public override void ComputePhysicsIntentions(Blackboard externalBlackboard = null)
        {
            base.ComputePhysicsIntentions(externalBlackboard);

            float currentProgress = _refBlackboard.GetValue<float>("CurrentProgress");
            isComplete = currentProgress >= 1.0f || currentProgress <= 0.0f;

            if (!_refBlackboard.ContainsKey("CurrentPath"))
            {
                currentPath = null;
            }
            else
            {
                currentPath = _refBlackboard.GetValue<GrindPath>("CurrentPath");
            }

            if (!isComplete || currentPath == null)
            {
                _refBlackboard.SetValue("GrindEntered", false);
                return;
            }
        }

        public override void OnEnter()
        {
            _refBlackboard.SetValue("Entered", true);
        }

        public override void OnExit()
        {
            _refBlackboard.SetValue("GrindEntered", false);
            _refBlackboard.Remove("CurrentPath");

            Parent.GetComponent<RideMovementController>().SetGrindPath(null);
            _grindCooldownTimer = GrindCooldownDuration;
            timerReady = false;
        }
    }

    public class NoRBStopAccelerationPastMaxSpeed : DataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard blackboard)
        {
            float currentSpeed = blackboard.GetValue<float>("CurrentSpeed");
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");
            float acceleration = blackboard.GetValue<float>("FinalAcceleration");
            bool isBraking = blackboard.GetValue<bool>("IsBraking");

            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;
            if (wasOverMaxSpeed && !isBraking)
                acceleration *= 0.0f;

            blackboard.SetValue("FinalAcceleration", acceleration);
            blackboard.SetValue("WasOverMaxSpeed", wasOverMaxSpeed);

            return blackboard;
        }
    }

    public class NoRBFakeGravityAccelerationFeature : DataPipelineStep<Blackboard>
    {
        public Vector3 gravityDirection = Vector3.down;
        public override Blackboard OnStep(Blackboard blackboard)
        {
            float acceleration = blackboard.GetValue<float>("FinalAcceleration");
            GrindPath grindPath = blackboard.GetValue<GrindPath>("CurrentPath");
            float currentProgress = blackboard.GetValue<float>("CurrentProgress");
            int progressDirection = blackboard.GetValue<int>("Direction");

            Spline path = grindPath.SplineContainer.Spline;

            float3 tangent = path.EvaluateTangent(currentProgress);
            Vector3 forward = grindPath.SplineContainer.transform.TransformDirection(tangent * progressDirection);
            forward.Normalize();


            if (forward.y < 0f)
            {
                float dotProduct = Vector3.Dot(gravityDirection, forward);
                float gravityAcceleration = Physics.gravity.magnitude * dotProduct;
                acceleration += gravityAcceleration;
            }

            blackboard.SetValue("FinalAcceleration", acceleration);

            return blackboard;
        }
    }

    public class NoRBScaleToFixedDeltaTime : DataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard blackboard)
        {
            float acceleration = blackboard.GetValue<float>("FinalAcceleration");
            float currentSpeed = blackboard.GetValue<float>("CurrentSpeed");
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");

            float accelInput = input.Accelerate - input.Brake;

            currentSpeed += acceleration * Time.fixedDeltaTime * accelInput;

            blackboard.SetValue("CurrentSpeed", currentSpeed);

            return blackboard;
        }
    }

    public class NoRBMinimumSpeedFeature : DataPipelineStep<Blackboard>
    {
        float MinimumGrindingSpeed = 2f;

        public override Blackboard OnStep(Blackboard blackboard)
        {
            float currentSpeed = blackboard.GetValue<float>("CurrentSpeed");

            currentSpeed = Mathf.Max(currentSpeed, MinimumGrindingSpeed);

            blackboard.SetValue("CurrentSpeed", currentSpeed);

            return blackboard;
        }
    }
}