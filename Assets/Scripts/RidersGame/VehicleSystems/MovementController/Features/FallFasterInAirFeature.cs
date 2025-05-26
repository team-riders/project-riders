using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{

    public class FallFasterInAirFeature : DataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard data)
        {
            float GroundPercent = data.GetValue<float>("GroundPercent");
            Rigidbody rigidbody = data.GetValue<Rigidbody>("Rigidbody");
            VehicleStats stats = data.GetValue<VehicleStats>("VehicleStats");
            // while in the air, fall faster
            if (GroundPercent <= 0f)
            {
                rigidbody.linearVelocity += stats.AddedGravity * Time.fixedDeltaTime * Physics.gravity;
            }

            return data;
        }
    }
}