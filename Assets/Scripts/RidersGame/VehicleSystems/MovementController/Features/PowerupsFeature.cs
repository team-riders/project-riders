using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.VehicleSystem
{
    public class PowerupsFeature : ConditionalDataPipelineStep<Blackboard>
    {
        public override Blackboard OnStep(Blackboard blackboard)
        {
            // change vehicle stats based on powerup controller changes
            // i am fully aware that none of this is physics. i genuinely do not think there is another way to do this
            PowerupController powerupController = blackboard.GetValue<PowerupController>("PowerupController");
            VehicleStats vehicleStats = blackboard.GetValue<VehicleStats>("VehicleStats");

            VehicleStats computedStats = powerupController.ReturnComputedStats();

            powerupController.TickPowerups();

            Debug.Log("vehicleStats.TopSpeed: " + vehicleStats.TopSpeed);
            Debug.Log("vehicleStats.Acceleration: " + vehicleStats.Acceleration);
            Debug.Log("computedStats.TopSpeed: " + computedStats.TopSpeed);
            Debug.Log("computedStats.Acceleration: " + computedStats.Acceleration);

            blackboard.SetValue("VehicleStats", computedStats);
            blackboard.SetValue("PowerupController", powerupController);

            return blackboard;
        }
    }
}
