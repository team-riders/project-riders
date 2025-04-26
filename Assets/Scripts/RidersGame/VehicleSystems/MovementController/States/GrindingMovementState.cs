using UnityEngine.Splines;
using UnityEngine;
using Unity.Mathematics;
using RidersGame.VehicleSystem;

namespace RidersRuntime.VehicleSystem
{
    public class GrindingMovementState : MovementStateAlt
    {
        public float feetOffsetY = 0.9f;
        public float autoGrindRadius = 1.5f;
        public float rotationSpeed = 10f;
        public float minimumDownwardSpeed = 0.5f;
        private float currentProgress;

        [Header("Debug")]
        public bool showGizmo = true;
        private GrindDetectorTrigger detector;

        private float progressRate;
        private float cooldownTimer = 0f;
        private bool isGrinding = false;

        private GrindPath currentPath;

        public bool IsGrinding => isGrinding && currentPath != null;
        SplineContainer _path;

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
        }
    }
}