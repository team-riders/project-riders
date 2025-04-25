using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    class BoardVehicle : BaseVehicle
    {
        //// replace base stats
        //public new VehicleStats baseStats = new VehicleStats
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

        [Header("Skateboard Specific Settings")]
        // Nothing for now

        [Header("Physical Wheels")]
        [Tooltip("The physical representations of the skateboard's wheels.")]
        public WheelCollider FrontLeftWheel;
        public WheelCollider FrontRightWheel;
        public WheelCollider BackLeftWheel;
        public WheelCollider BackRightWheel;

    }
}
