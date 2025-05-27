using RidersRuntime.VehicleSystem;
using UnityEngine;

namespace RidersRuntime.RaceManager
{
    public class RaceUIController : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Assign the InGameHUD GameObject here.")]
        public GameObject inGameHUD;

        // Local player reference to fetch UI data
        private GetPlayerInfo localPlayerInfo;

        void Start()
        {
            // Disable HUD initially
            if (inGameHUD != null)
                inGameHUD.SetActive(false);

            // Search for the local player's GetPlayerInfo component
            GetPlayerInfo[] allPlayers = FindObjectsOfType<GetPlayerInfo>();
            foreach (var player in allPlayers)
            {
                RacerComponent racer = player.GetComponent<RacerComponent>();
                if (racer != null && racer.isPlayer)
                {
                    localPlayerInfo = player;
                    Debug.Log($"Found local player: {player.name}");
                    break;
                }
            }

            // Handle case where no player was found
            if (localPlayerInfo == null)
            {
                Debug.LogWarning("[RaceUIController] No local GetPlayerInfo with RacerComponent.isPlayer = true");
            }
            else
            {
                ShowHUD(); // Enable HUD after finding player
            }
        }

        // Public method to show HUD
        public void ShowHUD()
        {
            if (inGameHUD != null)
            {
                inGameHUD.SetActive(true);
                Debug.Log("[RaceUIController] HUD shown.");
            }
            else
            {
                Debug.LogWarning("[RaceUIController] InGameHUD reference is missing.");
            }
        }

        // Fetch player info for the HUD to display
        public PlayerUIInfo GetPlayerUIInfo(int index)
        {
            if (localPlayerInfo == null)
            {
                Debug.LogWarning("[RaceUIController] Returning default PlayerUIInfo.");
                return new PlayerUIInfo();
            }

            var info = localPlayerInfo.GetPlayerInfoBundle();

            Debug.Log($"[RaceUIController] HUD Data → Pos: {info.Position}, Lap: {info.Lap}, Boost: {info.Boost}, Speed: {info.Speed}");

            return new PlayerUIInfo
            {
                position = info.Position,
                lap = info.Lap,
                time = info.Time,
                boost = info.Boost,
                isUsingBoost = info.IsUsingBoost,
                speed = info.Speed
            };
        }
    }

    // Struct used to pass HUD data cleanly
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
