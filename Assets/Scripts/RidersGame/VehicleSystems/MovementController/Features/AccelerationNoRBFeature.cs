using RidersRuntime.Data;
using RidersRuntime.Input;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    /// <summary>
    /// Needs blackboard information about InputData, RigidBody and VehicleStats.
    /// Sets a FinalAcceleration value in the blackboard.
    /// </summary>
    public class AccelerationNoRBFeature : DataPipelineStep<Blackboard>
    {
        const float accelerationCurveCoeff = 5;
        const float minimumGrindingSpeed = 2f;

        public override Blackboard OnStep(Blackboard blackboard)
        {
            ActorInputData input = blackboard.GetValue<ActorInputData>("InputData");
            VehicleStats stats = blackboard.GetValue<VehicleStats>("VehicleStats");
            int direction = blackboard.GetValue<int>("Direction");
            float currentSpeed = blackboard.GetValue<float>("CurrentSpeed");

            float Accelerate = input.Accelerate;
            float Brake = input.Brake;

            float accelInput = Accelerate - Brake;
            //Debug.Log("accelInput: " + accelInput);

            bool accelDirectionIsFwd = accelInput >= 0;
            //Debug.Log("accelDirectionIsFwd: " + accelDirectionIsFwd);
            bool localVelDirectionIsFwd = direction >= 0;

            // use the max speed for the direction we are going--forward or reverse.
            float maxSpeed = localVelDirectionIsFwd ? stats.TopSpeed : stats.ReverseSpeed;
            float accelPower = accelDirectionIsFwd ? stats.Acceleration : stats.ReverseAcceleration;
            //Debug.Log("accelPower: " + accelPower);

            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = stats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);

            bool isBraking = (localVelDirectionIsFwd && Brake > 0) || (!localVelDirectionIsFwd && Accelerate > 0);

            // if we are braking (moving reverse to where we are going)
            // use the braking acceleration instead
            float finalAccelPower = isBraking ? stats.Braking : accelPower;

            float finalAcceleration = finalAccelPower * accelRamp;

            // Actually generate the speed here
            currentSpeed += finalAcceleration * Time.fixedDeltaTime * accelInput;


            currentSpeed = Mathf.Max(currentSpeed, minimumGrindingSpeed);

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