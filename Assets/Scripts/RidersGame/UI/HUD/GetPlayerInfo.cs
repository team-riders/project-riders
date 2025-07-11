using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(BoostController))]
    public class GetPlayerInfo : MonoBehaviour
    {
        private BoostController boostController;
        private RideMovementController rideMovementController;
        private VehicleStats vehicleStats;

        void Start()
        {
            boostController = GetComponent<BoostController>();
            if (boostController == null)
            {
                Debug.LogError("BoostController component not found on this GameObject.");
                return;
            }
            else
            {
                vehicleStats = boostController.GetVehicleStats();
            }

            rideMovementController = GetComponent<RideMovementController>();
            if (rideMovementController == null)
            {
                Debug.LogError("RideMovementController component not found on this GameObject.");
                return;
            }
        }

        public PlayerInfo GetPlayerInfoBundle()
        {
            return new PlayerInfo
            {
                Position = 1, // TODO: Implement actual logic
                Lap = 1,      // TODO: Implement actual logic
                Time = 1000f, // TODO: Implement actual logic
                Boost = boostController.GetBoostGauge(),
                BoostGaugeMax = boostController.GetBoostGaugeMax(),
                IsUsingBoost = boostController.IsUsingBoost(),
                JumpCharge = rideMovementController.GetCurrentJumpCharge(),
                JumpChargeMinScale = rideMovementController.GetJumpChargeMinScale()
            };
        }

        public class PlayerInfo
        {
            public int Position { get; set; }
            public int Lap { get; set; }
            public float Time { get; set; }
            public float Boost { get; set; }
            public float BoostGaugeMax { get; set; }
            public bool IsUsingBoost { get; set; }
            public float JumpCharge { get; set; }
            public float JumpChargeMinScale { get; set; }
        }
    }
}
