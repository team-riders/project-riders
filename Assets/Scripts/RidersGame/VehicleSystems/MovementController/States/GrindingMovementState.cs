using UnityEngine;
using RidersGame.VehicleSystem;

namespace RidersRuntime.VehicleSystem
{
    public class GrindingMovementState : MovementStateAlt
    {
        public float minimumDownwardSpeed = 0.5f;
        public const float GrindCooldownDuration = 5f;
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
                new GrindingFeature(),
                new JumpFeature(() => true),
                new GrindExitFeature(),
            });
        }

        public override void ComputePhysicsIntentions(Blackboard externalBlackboard = null)
        {
            base.ComputePhysicsIntentions(externalBlackboard);

            float currentProgress = _refBlackboard.GetValue<float>("CurrentProgress");
            isComplete = currentProgress >= 1.0f || currentProgress <= 0.0f;
            currentPath = _refBlackboard.GetValue<GrindPath>("CurrentPath");

            if (!isComplete || currentPath == null)
            {
                _refBlackboard.SetValue("GrindEntered", false);
                _refBlackboard.Remove("CurrentPath");
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
}