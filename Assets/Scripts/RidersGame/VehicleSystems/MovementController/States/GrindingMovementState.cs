using UnityEngine;
using RidersGame.VehicleSystem;

namespace RidersRuntime.VehicleSystem
{
    public class GrindingMovementState : MovementStateAlt
    {
        public float minimumDownwardSpeed = 0.5f;
        private bool isGrinding = false;

        private GrindPath currentPath;

        public bool IsGrinding => isGrinding && currentPath != null;

        public const float GrindCooldownDuration = 0.5f;

        private float _grindCooldownTimer = 0f;

        public bool CanGrind => _grindCooldownTimer <= 0f;

        public void DoCooldown()
        {
            if (_grindCooldownTimer > 0)
            {
                _grindCooldownTimer -= Time.fixedDeltaTime;
            }
            else if (_grindCooldownTimer < 0)
            {
                _grindCooldownTimer = 0;
            }
        }

        public GrindingMovementState() : base("Grinding Movement State")
        {
            _featureQueue = new(new()
            {
                new GrindEnterFeature(),
                new AccelerationFeature(),
                new GrindingFeature(),
                new JumpFeature(),
                new GrindExitFeature(),
            });
        }

        public override void ComputeIntention(Blackboard externalBlackboard = null)
        {
            base.ComputeIntention(externalBlackboard);

            isGrinding = _refBlackboard.GetValue<bool>("IsGrinding");
            currentPath = _refBlackboard.GetValue<GrindPath>("CurrentPath");

            if (!isGrinding || currentPath == null)
            {
                _refBlackboard.SetValue("GrindEntered", false);
                _refBlackboard.Remove("CurrentPath");
                return;
            }
        }

        public override void OnEnter()
        {
            _refBlackboard.SetValue("Entered", true);
            _refBlackboard.SetValue("IsGrinding", true);
            isGrinding = true;
        }

        public override void OnExit()
        {
            _refBlackboard.SetValue("GrindEntered", false);
            _refBlackboard.Remove("CurrentPath");

            _grindCooldownTimer = GrindCooldownDuration;
        }
    }
}