using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class EchoState : MovementStateAlt
    {
        public EchoState() : base("Echo State")
        {
        }

        public override void OnUpdate()
        {
            Debug.Log("EchoState OnUpdate called.");
        }

        public override void ComputePhysicsIntentions(Blackboard blackboard)
        {
            Debug.Log("EchoState ComputeIntention called.");
        }
    }
}