using RidersRuntime.Data;
using RidersRuntime.VehicleSystem;
using UnityEngine;

namespace RidersRuntime.RaceManager
{
    [RequireComponent(typeof(RideMovementController))]
    [RequireComponent(typeof(BoostController))]
    public class RacerComponent : MonoBehaviour
    {
        [Header("Racer Info")]
        public int racerID;
        public RiderConfig rider;
        public bool isPlayer;

        private VehicleType vehicleType;

        // Component references
        private RideMovementController movementController;
        private BoostController boostController;

        // Runtime state
        private float currentSpeed;
        private float currentBoost;
        private bool usingBoost;
        private int currentLap;
        private int racePosition;

        void Start()
        {
            movementController = GetComponent<RideMovementController>();
            boostController = GetComponent<BoostController>();

            SetupRider(new RiderSelection
            {
                rider = rider,
                vehicleType = vehicleType,
                isPlayer = isPlayer
            });
        }

        public void SetupRider(RiderSelection newRider)
        {
            rider = newRider.rider;
            isPlayer = newRider.isPlayer;
            vehicleType = newRider.vehicleType;

            if (rider == null)
            {
                Debug.LogError("[RacerComponent] RiderConfig is not assigned on: " + gameObject.name);
                return;
            }

            InitializeDefaultState();
        }

        private void InitializeDefaultState()
        {
            currentLap = 1;
            currentSpeed = 0f;
            currentBoost = 100f;
            usingBoost = false;
            racePosition = 0;
        }

        void Update()
        {
            // Use real data if available
            currentSpeed = movementController != null ? movementController.GetCurrentSpeed() : 0f;
            currentBoost = boostController != null ? boostController.GetBoostGauge() : 0f;
            usingBoost = boostController != null && boostController.IsUsingBoost();
        }

        // Public accessors
        public float GetSpeed() => currentSpeed;
        public float GetBoostAmount() => currentBoost;
        public bool IsUsingBoost() => usingBoost;
        public int GetCurrentLap() => currentLap;
        public int GetRacePosition() => racePosition;
    }
}
