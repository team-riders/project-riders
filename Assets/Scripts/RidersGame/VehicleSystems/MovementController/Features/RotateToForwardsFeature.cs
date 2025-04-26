using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class RotateToForwardsFeature : IDataPipelineStep<Blackboard>
    {
        const float angularVelocitySteering = 0.4f;
        const float angularVelocitySmoothSpeed = 20f;
        const float velocitySteering = 25f;

        public Blackboard ProcessData(Blackboard blackboard)
        {
            VehicleStats stats = blackboard.GetValue<VehicleStats>("VehicleStats");
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");

            bool localVelDirectionIsFwd = blackboard.GetValue<bool>("LocalVelDirectionIsFwd");
            bool accelDirectionIsFwd = blackboard.GetValue<bool>("AccelDirectionIsFwd");
            float turningPower = blackboard.GetValue<float>("TurningPower");

            float m_CurrentGrip = stats.Grip;
            Vector3 localVel = rb.transform.InverseTransformVector(rb.linearVelocity);

            float angularVelocitySteerStrength = angularVelocitySteering;
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                angularVelocitySteerStrength *= -1.0f;

            float rotationAngle = turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime;
            Vector3 turnVelocity = Quaternion.AngleAxis(rotationAngle, rb.transform.up) * rb.linearVelocity;

            Vector3 angularVelocity = rb.angularVelocity;
            angularVelocity.y = Mathf.MoveTowards(angularVelocity.y, turningPower * angularVelocitySteerStrength, angularVelocitySmoothSpeed * Time.fixedDeltaTime);

            rb.linearVelocity = turnVelocity;
            rb.angularVelocity = angularVelocity;

            return blackboard;
        }
    }
}