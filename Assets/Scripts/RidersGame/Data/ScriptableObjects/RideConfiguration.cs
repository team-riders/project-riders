using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.Data
{
    [CreateAssetMenu(fileName = "Ride Configuration", menuName = "Project Riders/New Ride Configuration", order = 11)]
    public class RideConfiguration : ScriptableObject
    {

        public VehicleType VehicleType = VehicleType.Skateboard;
        public VehicleStats VehicleStats = new VehicleStats()
        {
            TopSpeed = 7,
            Acceleration = 2,
            ReverseSpeed = 1,
            ReverseAcceleration = 0.25f,
            AccelerationCurve = 0.5f,
            Braking = 10,
            CoastingDrag = 1,
            Grip = 0.95f,
            Steer = 5,
            AddedGravity = -0.1f,
            BoostTopSpeed = 80,
            BoostAccel = 15,
        };

    }
}