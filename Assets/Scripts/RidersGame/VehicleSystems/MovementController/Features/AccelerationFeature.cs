using RidersRuntime.Data;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    /// <summary>
    /// Needs blackboard information about InputData, RigidBody and VehicleStats.
    /// Sets a FinalAcceleration value in the blackboard.
    /// </summary>
    public class AccelerationFeature : IDataPipelineStep<Blackboard>
    {
        public Blackboard ProcessData(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            Rigidbody rb = blackboard.GetValue<Rigidbody>("Rigidbody");
            VehicleStats stats = blackboard.GetValue<VehicleStats>("VehicleStats");
            float Accelerate = input.Accelerate;
            float Brake = input.Brake;
            Transform currentTransform = rb.transform;

            Vector3 linearVelocity = rb.linearVelocity;

            float accelInput = Accelerate - Brake;
            //Debug.Log("accelInput: " + accelInput);

            // manual acceleration curve coefficient scalar
            float accelerationCurveCoeff = 5;
            Vector3 localVel = currentTransform.InverseTransformVector(linearVelocity);

            bool accelDirectionIsFwd = accelInput >= 0;
            //Debug.Log("accelDirectionIsFwd: " + accelDirectionIsFwd);
            bool localVelDirectionIsFwd = localVel.z >= 0;

            // use the max speed for the direction we are going--forward or reverse.
            float maxSpeed = localVelDirectionIsFwd ? stats.TopSpeed : stats.ReverseSpeed;
            float accelPower = accelDirectionIsFwd ? stats.Acceleration : stats.ReverseAcceleration;
            //Debug.Log("accelPower: " + accelPower);

            float currentSpeed = linearVelocity.magnitude;
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = stats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            bool isBraking = (localVelDirectionIsFwd && Brake > 0) || (!localVelDirectionIsFwd && Accelerate > 0);

            // if we are braking (moving reverse to where we are going)
            // use the braking acceleration instead
            float finalAccelPower = isBraking ? stats.Braking : accelPower;

            float finalAcceleration = finalAccelPower * accelRamp;

            blackboard.SetValue("FinalAcceleration", finalAcceleration);
            blackboard.SetValue("CurrentSpeed", currentSpeed);
            blackboard.SetValue("MaxSpeed", maxSpeed);
            blackboard.SetValue("IsBraking", isBraking);
            blackboard.SetValue("AccelDirectionIsFwd", accelDirectionIsFwd);
            blackboard.SetValue("LocalVelDirectionIsFwd", localVelDirectionIsFwd);

            return blackboard;
        }
    }
}