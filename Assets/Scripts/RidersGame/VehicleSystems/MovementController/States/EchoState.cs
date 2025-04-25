using RidersRuntime.Input;
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

        public override void ComputeIntention(ActorInputData input, out Vector3 intent_velocity, out Vector3 intent_rotation)
        {
            Debug.Log("EchoState ComputeIntention called.");
            intent_velocity = Vector3.zero;
            intent_rotation = Vector3.zero;
        }
    }
}