using RidersRuntime.Data;
using RidersRuntime.VehicleSystem;
using UnityEngine;

namespace RidersRuntime
{
    public class BoostController
    {
        public PowerupController powerupController { get; set; }
        VehicleStats boostStats;
        StatPowerup boostPowerup;

        float boostGauge;
        float boostGaugeMax;
        float boostGaugePerCharge;

        public BoostController(PowerupController powerupController)
        {
            this.powerupController = powerupController;

            boostStats = new()
            {
                TopSpeed = powerupController.BaseStats.BoostTopSpeed,
                Acceleration = powerupController.BaseStats.BoostAccel,
                AccelerationCurve = 0.4f
            };

            boostPowerup = new()
            {
                modifiers = boostStats,
                PowerUpID = "boost",
                ElapsedTime = 0,
                MaxTime = 5,
            };

            boostGaugePerCharge = powerupController.BaseStats.BoostGaugePerCharge;
            boostGaugeMax = powerupController.BaseStats.BoostGaugeMax;
            boostGauge = boostGaugePerCharge;

            Debug.Log("boostStats: " + boostStats);
            Debug.Log("boostPowerup: " + boostPowerup);
            Debug.Log("boostGaugePerCharge: " + boostGaugePerCharge);
            Debug.Log("boostGaugeMax: " + boostGaugeMax);
            Debug.Log("boostGauge: " + boostGauge);
        }

        // applies boost
        public bool ApplyBoost()
        {
            Debug.Log("boostGauge >= boostGaugePerCharge: " + (boostGauge >= boostGaugePerCharge));
            if (boostGauge >= boostGaugePerCharge)
            {
                Debug.Log("!powerupController.IsInList(boostPowerup.PowerUpID): " + !powerupController.IsInList(boostPowerup.PowerUpID));
                if (!powerupController.IsInList(boostPowerup.PowerUpID))
                {
                    powerupController.AddPowerup(boostPowerup);
                    
                    //DecreaseGauge(boostGaugePerCharge);

                    return true;
                }
            }
            return false;
        }

        public void IncreaseGauge(float increase)
        {
            boostGauge += increase;
            Mathf.Clamp(boostGauge, 0, boostGaugeMax);
        }

        public void DecreaseGauge(float decrease)
        {
            boostGauge -= decrease;
            Mathf.Clamp(boostGauge, 0, boostGaugeMax);
        }
    }
}
