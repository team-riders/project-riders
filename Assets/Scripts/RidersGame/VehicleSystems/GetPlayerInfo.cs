using RidersRuntime.Data;
using RidersRuntime.RaceManager;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(BoostController))]
    [RequireComponent(typeof(RacerComponent))]
    public class GetPlayerInfo : MonoBehaviour
    {
        private BoostController boostController;
        private RacerComponent racerComponent;

        // Local race start timestamp
        private float raceStartTime;

        void Start()
        {
            boostController = GetComponent<BoostController>();
            racerComponent = GetComponent<RacerComponent>();

            if (boostController == null)
            {
                Debug.LogError("[GetPlayerInfo] Missing BoostController on: " + gameObject.name);
            }

            if (racerComponent == null)
            {
                Debug.LogError("[GetPlayerInfo] Missing RacerComponent on: " + gameObject.name);
            }

            // Capture scene-based "race start" timestamp
            raceStartTime = Time.timeSinceLevelLoad;
        }

        /// <summary>
        /// Returns a bundle of current player UI data for HUD display.
        /// </summary>
        public PlayerInfo GetPlayerInfoBundle()
        {
            return new PlayerInfo
            {
                Position = racerComponent?.GetRacePosition() ?? 0,
                Lap = racerComponent?.GetCurrentLap() ?? 0,
                Time = Time.timeSinceLevelLoad - raceStartTime,
                Boost = boostController?.GetBoostGauge() ?? 0f,
                BoostGaugeMax = boostController?.GetBoostGaugeMax() ?? 100f,
                IsUsingBoost = boostController?.IsUsingBoost() ?? false,
                Speed = racerComponent?.GetSpeed() ?? 0f
            };
        }
    }

    /// <summary>
    /// Data structure returned to the UI controller to populate player HUD values.
    /// Suggest moving to a separate file if reused elsewhere.
    /// </summary>
    public class PlayerInfo
    {
        public int Position { get; set; }
        public int Lap { get; set; }
        public float Time { get; set; }
        public float Boost { get; set; }
        public float BoostGaugeMax { get; set; }
        public bool IsUsingBoost { get; set; }
        public float Speed { get; set; }
    }
}
