using RidersRuntime.Data;
using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class PowerupController
    {
        List<StatPowerup> m_ActivePowerupList = new();

        public VehicleStats BaseStats { get; set; } = new();
        VehicleStats computedStats = new();
        public VehicleStats GetCurrentStats() => computedStats;

        public PowerupController(VehicleStats vehicleStats)
        {
            BaseStats = vehicleStats;
            computedStats = BaseStats;
        }

        // adds powerup to list
        public void AddPowerup(StatPowerup powerup)
        {
            if (!IsInList(powerup.PowerUpID))
            {
                m_ActivePowerupList.Add(powerup);
            }
        }

        // checks if powerup ID is in list
        public bool IsInList(string powerupID)
        {
            foreach (StatPowerup powerup in m_ActivePowerupList)
            {
                if (powerup.PowerUpID == powerupID) return true;
            }
            return false;
        }

        public void TickPowerups()
        {
            m_ActivePowerupList.RemoveAll(powerup => powerup.ElapsedTime >= powerup.MaxTime);

            VehicleStats stats = new();

            foreach (StatPowerup powerup in m_ActivePowerupList)
            {
                powerup.ElapsedTime += Time.fixedDeltaTime;
                stats += powerup.modifiers;
            }

            computedStats = stats + BaseStats;

            PostProcessStats();
        }

        public void PostProcessStats()
        {
            computedStats.Grip = Mathf.Clamp(computedStats.Grip, 0.0f, 1.0f);
        }

        public VehicleStats ReturnComputedStats()
        {
            return computedStats;
        }
    }
}