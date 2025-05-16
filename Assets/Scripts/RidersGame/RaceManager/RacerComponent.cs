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

        VehicleType vehicleType;

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
            // SetupVisuals();
        }

        void SetupVisuals()
        {
            // Character visuals
            GameObject visualPrefab = rider.characterModelPrefab;
            GameObject visualObj = Instantiate(visualPrefab, transform);
            visualObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Ride visuals
        }

        void InitializePlayerSettings()
        {
        }

        void InitializeAISettings()
        {

        }
    }
}