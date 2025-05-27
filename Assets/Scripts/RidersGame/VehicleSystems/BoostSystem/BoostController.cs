using RidersRuntime.Data;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(RideMovementController))]
    public class BoostController : MonoBehaviour
    {
        public RideMovementController controller;
        private VehicleStats boostStats;

        private float boostGauge;
        private float boostGaugeMax;
        private float boostGaugePerCharge;

        private IInput iInput;
        private ActorInputData inputData;

        public bool needsGauge = false;

        public void TurnOn() => needsGauge = true;
        public void TurnOff() => needsGauge = false;
        public bool IsOn() => needsGauge;

        void Start()
        {
            controller = GetComponent<RideMovementController>();
            iInput = GetComponent<BaseInput>();

            boostStats = new VehicleStats
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
            Debug.Log("boostGaugePerCharge: " + boostGaugePerCharge);
            Debug.Log("boostGaugeMax: " + boostGaugeMax);
            Debug.Log("boostGauge: " + boostGauge);
        }

        public void ApplyBoost()
        {
            if (boostGauge >= boostGaugePerCharge)
            {
                StatPowerup boostPowerup = new StatPowerup
                {
                    modifiers = boostStats,
                    PowerUpID = "boost",
                    ElapsedTime = 0,
                    MaxTime = 1,
                };

                PowerupController powerupController = controller.powerupController;

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
            boostGauge = Mathf.Clamp(boostGauge + increase, 0, boostGaugeMax);
        }

        public void DecreaseGauge(float decrease)
        {
            boostGauge = Mathf.Clamp(boostGauge - decrease, 0, boostGaugeMax);
        }

        void Update()
        {
            inputData = iInput.GrabCurrentFrameInputs();
        }

        void FixedUpdate()
        {
            if (inputData.BoostRam)
            {
                ApplyBoost();
            }
        }

        public VehicleStats GetVehicleStats() => boostStats;
        public float GetBoostGauge() => boostGauge;
        public float GetBoostGaugeMax() => boostGaugeMax;
        public bool IsUsingBoost() => inputData.BoostRam && boostGauge >= boostGaugePerCharge;
    }
}
