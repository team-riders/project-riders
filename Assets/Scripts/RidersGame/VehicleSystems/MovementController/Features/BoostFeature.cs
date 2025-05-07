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

            BoostController boostController = blackboard.GetValue<BoostController>("BoostController");


            Debug.Log("input.BoostRam " + input.BoostRam);
            if (input.BoostRam)
            {
                Debug.Log("ApplyBoost: " + boostController.ApplyBoost());
                if (boostController.ApplyBoost())
                {
                    Debug.Log("applying rigidbody forward force");
                    rigidbody.AddForce(rigidbody.transform.forward * 500, ForceMode.Impulse);
                }
            }

            // nothing in the blackboard ends up actually changing, run by james
            blackboard.SetValue("Rigidbody", rigidbody);
            return blackboard;
        }

        public BoostFeature(Func<bool> condition = null) : base(condition)
        {
        }
    }
}
