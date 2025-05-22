using UnityEngine;

namespace RidersRuntime.RaceManager
{
    public class RaceManager : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Drag the InGameHUD GameObject here.")]
        public GameObject inGameHUD;

        private RacerComponent localPlayerRacer;

        void Start()
        {
            // Hide the HUD at the start of the scene
            if (inGameHUD != null)
            {
                inGameHUD.SetActive(false);
            }

            // Try to find the local player's RacerComponent
            RacerComponent[] racers = FindObjectsOfType<RacerComponent>();
            foreach (var racer in racers)
            {
                if (racer.isPlayer)
                {
                    localPlayerRacer = racer;
                    break;
                }
            }

            if (localPlayerRacer == null)
            {
                Debug.LogWarning("Local player RacerComponent not found.");
            }
        }

        /// <summary>
        /// Call this method once loading is finished and the scene is ready.
        /// This will activate the InGameHUD.
        /// </summary>
        public void ShowHUD()
        {
            if (inGameHUD != null)
            {
                inGameHUD.SetActive(true);
            }
            else
            {
                Debug.LogWarning("HUD reference not assigned on RaceManager.");
            }
        }

        /// <summary>
        /// Provides real-time player UI data from their RacerComponent.
        /// </summary>
        public PlayerUIInfo GetPlayerUIInfo(int index)
        {
            if (localPlayerRacer == null)
            {
                return new PlayerUIInfo(); // return default
            }

            return new PlayerUIInfo
            {
                position = localPlayerRacer.GetRacePosition(),
                lap = localPlayerRacer.GetCurrentLap(),
                time = Time.timeSinceLevelLoad,
                boost = localPlayerRacer.GetBoostAmount(),
                isUsingBoost = localPlayerRacer.IsUsingBoost(),
                speed = localPlayerRacer.GetSpeed()
            };
        }
    }

    public struct PlayerUIInfo
    {
        public int position;
        public int lap;
        public float time;
        public float boost;
        public bool isUsingBoost;
        public float speed;
    }
}
