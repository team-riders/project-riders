using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{

    public class JumpFeature : IDataPipelineStep<Blackboard>
    {
        public Blackboard ProcessData(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            float GroundPercent = blackboard.GetValue<float>("GroundPercent");
            bool WantsToJump = input.Jump;
            float WantsToHold = input.JumpHoldDuration;
            float maxSpeed = blackboard.GetValue<float>("MaxSpeed");

            // Config
            float JumpForce = 0;

            // Local
            float JumpCharge = 0;


            if (WantsToHold > 0 && GroundPercent > 0.0f)
            {
                JumpCharge = Mathf.Clamp(WantsToHold / 120f, 0.5f, 1.0f);
                Debug.Log("JumpCharge: " + JumpCharge);
                if (WantsToHold > 180)
                {
                    maxSpeed *= 0.5f;
                }
            }
            if (WantsToJump && GroundPercent > 0.0f)
            {
                Debug.Log("JumpForce * JumpCharge: " + JumpForce * JumpCharge);
                // Rigidbody.AddForce(Vector3.up * (JumpForce * JumpCharge), ForceMode.Impulse);


                blackboard.SetValue("WantsToJump", false);
                blackboard.SetValue("WantsToHold", 0);
                blackboard.SetValue("MaxSpeed", maxSpeed);
            }

            return blackboard;
        }
    }
}