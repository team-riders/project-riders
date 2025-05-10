
using UnityEngine;
namespace RidersRuntime.Data
{
    [CreateAssetMenu(fileName = "VehicleStats", menuName = "RidersGame/VehicleStats", order = 1)]
    public class VehicleStatsSO : ScriptableObject
    {
        public VehicleStats _vehicleStats = new();
    }
}