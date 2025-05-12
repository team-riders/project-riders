using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.RaceManager
{
    // We should use a prefab for each vehicle type because there are specific configs for each vehicle
    // We will work here before moving into the vehicle system
    public class RacerComponent : MonoBehaviour
    {
        public RiderConfig rider;
        public bool isPlayer;

        VehicleType vehicleType;

        void Start()
        {
            // Check if there is a rider assigned
            if (rider == null)
            {
                Debug.LogError("Rider is not assigned to the RacerComponent on " + gameObject.name);
                return;
            }

            if (isPlayer)
            {
                // Initialize player-specific settings
                InitializePlayerSettings();
            }
            else
            {
                // Initialize AI-specific settings
                InitializeAISettings();
            }
        }

        public void SetRider(RiderSelection newRider)
        {
            rider = newRider.rider;
            isPlayer = newRider.isPlayer;
            vehicleType = newRider.vehicleType;
        }

        void InitializePlayerSettings()
        {
        }

        void InitializeAISettings()
        {

        }
    }
}