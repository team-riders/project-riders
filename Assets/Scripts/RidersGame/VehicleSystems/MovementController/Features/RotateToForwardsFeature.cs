using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class RotateToForwardsFeature : IDataPipelineStep<Blackboard>
    {
        float angularVelocitySteering = 0.4f;
        float angularVelocitySmoothSpeed = 20f;
        float velocitySteering = 20f;

        public Blackboard ProcessData(Blackboard blackboard)
        {
            bool localVelDirectionIsFwd = blackboard.GetValue<bool>("LocalVelDirectionIsFwd");
            bool accelDirectionIsFwd = blackboard.GetValue<bool>("AccelDirectionIsFwd");
            float turningPower = blackboard.GetValue<float>("TurningPower");
            VehicleStats stats = blackboard.GetValue<VehicleStats>("VehicleStats");
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            Vector3 linearVelocity = blackboard.GetValue<Vector3>("IntentVelocity");

            float m_CurrentGrip = stats.Grip;

            Vector3 localVel = rb.transform.InverseTransformVector(rb.linearVelocity);

            if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                angularVelocitySteering *= -1.0f;

            Vector3 angularVelocity = rb.angularVelocity;

            angularVelocity.y = Mathf.MoveTowards(angularVelocity.y, turningPower * angularVelocitySteering, angularVelocitySmoothSpeed * Time.fixedDeltaTime);

            Vector3 newVelocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime, rb.transform.up) * linearVelocity;
            blackboard.SetValue("IntentVelocity", newVelocity);
            blackboard.SetValue("IntentRotation", angularVelocity);

            return blackboard;
        }
    }
}