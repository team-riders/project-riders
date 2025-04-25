using RidersRuntime.Data;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    /// <summary>
    /// Needs blackboard information about InputData, RigidBody, VehicleStats, FinalAcceleration, HasCollision and GroundPercent
    /// Sets "MovementVector" in the blackboard.
    /// </summary>
    public class SteerTurnFeature : IDataPipelineStep<Blackboard>
    {
        public Blackboard ProcessData(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            VehicleStats stats = blackboard.GetValue<VehicleStats>("VehicleStats");
            float finalAcceleration = blackboard.GetValue<float>("FinalAcceleration");

            float turnInput = input.TurnInput;
            float accelInput = input.Accelerate - input.Brake;

            Transform currentTransform = rb.transform;

            // ? has collision & ground percent can be updated every frame and polled
            bool m_HasCollision = blackboard.GetValue<bool>("HasCollision");
            float GroundPercent = blackboard.GetValue<float>("GroundPercent");


            float turningPower = turnInput * stats.Steer;

            Quaternion turnAngle = Quaternion.AngleAxis(turningPower, currentTransform.up);
            Vector3 fwd = turnAngle * currentTransform.forward;
            Vector3 movement = ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f) * accelInput * finalAcceleration * fwd;

            blackboard.SetValue("MovementVector", movement);
            blackboard.SetValue("TurningPower", turningPower);

            return blackboard;
        }
    }
}