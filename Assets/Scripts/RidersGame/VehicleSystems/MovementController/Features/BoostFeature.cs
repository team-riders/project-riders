using RidersRuntime.Data;
using RidersRuntime.Input;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class BoostFeature : ConditionalDataPipelineStep<Blackboard>
    {

        public override Blackboard OnStep(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            Rigidbody rigidbody = blackboard.GetValue<Rigidbody>("Rigidbody");


            float boostTopSpeed = blackboard.GetValue<float>("");   // figure out tag
            float boostAccel = blackboard.GetValue<float>("");

            PowerupController powerupController = blackboard.GetValue<PowerupController>("PowerupController");


            if (input.BoostRam)
            {
                VehicleStats boostStats = new()
                {
                    TopSpeed = boostTopSpeed,
                    Acceleration = boostAccel,
                    AccelerationCurve = 0.4f
                };

                StatPowerup boostPowerup = new()
                {
                    modifiers = boostStats,
                    PowerUpID = "boost",
                    ElapsedTime = 0,
                    MaxTime = 5,
                };

                powerupController.AddPowerup(boostPowerup);

                rigidbody.AddForce(Vector3.forward * 20, ForceMode.Impulse);
            }
        }

        public BoostFeature(Func<bool> condition = null) : base(condition)
        {
        }
    }
}
