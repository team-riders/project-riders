using UnityEngine;

namespace RidersCore.VehicleSystem
{
    class BladesVehicle : BaseVehicle
    {
        //// replace base stats
        //public new BaseVehicle.Stats baseStats = new BaseVehicle.Stats
        //{
        //    TopSpeed = 50f,
        //    Acceleration = 10f,
        //    AccelerationCurve = 4f,
        //    Braking = 10f,
        //    ReverseAcceleration = 5f,
        //    ReverseSpeed = 5f,
        //    Steer = 5f,
        //    CoastingDrag = 4f,
        //    Grip = .95f,
        //    AddedGravity = 1f,
        //    BoostTopSpeed = 80f,
        //    BoostAccel = 15f,
        //};

        [Header("Blades Specific Settings")]
        // Nothing for now

        [Header("Left Wheels")]
        [Tooltip("The physical representations of the blades' wheels on the left foot.")]
        public WheelCollider LFrontWheel;
        public WheelCollider LMiddleFrontWheel;
        public WheelCollider LMiddleBackWheel;
        public WheelCollider LBackWheel;

        [Header("Right Wheels")]
        [Tooltip("The physical representations of the blades' wheels on the left foot.")]
        public WheelCollider RFrontWheel;
        public WheelCollider RMiddleFrontWheel;
        public WheelCollider RMiddleBackWheel;
        public WheelCollider RBackWheel;
    }
}
