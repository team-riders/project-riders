using RidersRuntime.Data;
using RidersRuntime.VehicleSystem;
using UnityEngine;

namespace RidersRuntime
{
    public class TrailPowerup : MonoBehaviour
    {
        private VehicleStats _trailPowerupStats;
        private RideMovementController _rideController;
        private PowerupController _powerupController;
        // applies small speed powerup to players who travel through a trail
        void Start()
        {
            _trailPowerupStats = new()
            {
                TopSpeed = 20,
                Acceleration = 5,
                AccelerationCurve = 0.2f
            };
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            StatPowerup trailPowerup = new()
            {
                modifiers = _trailPowerupStats,
                PowerUpID = "trail",
                ElapsedTime = 0,
                MaxTime = 2
            };

            _rideController = other.gameObject.GetComponent<RideMovementController>();
            _powerupController = _rideController.powerupController;
            _powerupController.AddPowerup(trailPowerup);
        }
    }
}
