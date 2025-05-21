using RidersRuntime.Data;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(RideMovementController))]
    public class BoostController : MonoBehaviour
    {
        public RideMovementController controller;
        VehicleStats boostStats;

        float boostGauge;
        float boostGaugeMax;
        float boostGaugePerCharge;

        IInput iInput;
        ActorInputData inputData;

        public bool needsGauge = false;
        public void TurnOn() => needsGauge = true;
        public void TurnOff() => needsGauge = false;
        public bool IsOn() => needsGauge;

        void Start()
        {
            controller = GetComponent<RideMovementController>();
            iInput = GetComponent<BaseInput>();

            boostStats = new()
            {
                TopSpeed = controller.m_VehicleStats._vehicleStats.BoostTopSpeed,
                Acceleration = controller.m_VehicleStats._vehicleStats.BoostAccel,
                AccelerationCurve = 0.4f,
                ForceAccel = 1
            };

            boostGaugePerCharge = controller.m_VehicleStats._vehicleStats.BoostGaugePerCharge;
            boostGaugeMax = controller.m_VehicleStats._vehicleStats.BoostGaugeMax;
            boostGauge = boostGaugePerCharge;

            Debug.Log("boostStats: " + boostStats);
            //Debug.Log("boostPowerup: " + boostPowerup);
            Debug.Log("boostGaugePerCharge: " + boostGaugePerCharge);
            Debug.Log("boostGaugeMax: " + boostGaugeMax);
            Debug.Log("boostGauge: " + boostGauge);
        }

        // applies boost
        public void ApplyBoost()
        {
            Debug.Log("boostGauge >= boostGaugePerCharge: " + (boostGauge >= boostGaugePerCharge));
            if (boostGauge >= boostGaugePerCharge)
            {
                StatPowerup boostPowerup = new()
                {
                    modifiers = boostStats,
                    PowerUpID = "boost",
                    ElapsedTime = 0,
                    MaxTime = 1,
                };

                PowerupController powerupController = controller.powerupController;

                Debug.Log("!powerupController.IsInList(boostPowerup.PowerUpID): " + !powerupController.IsInList(boostPowerup.PowerUpID));
                if (!powerupController.IsInList(boostPowerup.PowerUpID))
                {
                    powerupController.AddPowerup(boostPowerup);

                    if (needsGauge)
                    {
                        DecreaseGauge(boostGaugePerCharge);
                    }
                }
            }
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

        private void Update()
        {
            inputData = iInput.GrabCurrentFrameInputs();
        }

        void FixedUpdate()
        {
            //Debug.Log("topspeed: " + controller.m_VehicleStats._vehicleStats.BoostTopSpeed);
            if (inputData.BoostRam)
            {
                Debug.Log("applying boost");
                ApplyBoost();
            }
        }
    }
}
