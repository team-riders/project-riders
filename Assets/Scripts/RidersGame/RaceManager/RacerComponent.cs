using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.RaceManager
{
    // We should use a prefab for each vehicle type because there are specific configs for each vehicle
    // We will work here before moving into the vehicle system
    public class RacerComponent : MonoBehaviour
    {
        public int racerID;
        public RiderConfig rider;
        public bool isPlayer;

        private VehicleType vehicleType;

        // Simulated runtime state (example placeholders for now)
        private float currentSpeed;
        private float currentBoost;
        private bool usingBoost;
        private int currentLap;
        private int racePosition;

        void Start()
        {
            SetupRider(new RiderSelection()
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
                Debug.LogError("Rider is not assigned to the RacerComponent on " + gameObject.name);
                return;
            }

            if (isPlayer)
            {
                InitializePlayerSettings();
            }
            else
            {
                InitializeAISettings();
            }
        }

        void InitializePlayerSettings()
        {
            // Set initial values
            currentLap = 1;
            currentSpeed = 0f;
            currentBoost = 100f;
            usingBoost = false;
            racePosition = 0;
        }

        void InitializeAISettings()
        {
            // Set AI-specific starting conditions (same for now)
            InitializePlayerSettings();
        }

        // These would be updated by your movement, boost, and lap tracking systems
        public float GetSpeed() => currentSpeed;
        public float GetBoostAmount() => currentBoost;
        public bool IsUsingBoost() => usingBoost;
        public int GetCurrentLap() => currentLap;
        public int GetRacePosition() => racePosition;

        // Temporary simulation update (optional testing)
        void Update()
        {
            // Simulate values
            currentSpeed = Mathf.PingPong(Time.time * 30f, 200f);
            currentBoost = Mathf.PingPong(Time.time * 15f, 100f);
            usingBoost = Mathf.Sin(Time.time) > 0.5f;
        }
    }
}
