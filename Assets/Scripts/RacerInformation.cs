using UnityEngine;
using UnityEngine.TextCore.Text;

public class RacerInformation : MonoBehaviour
{
    // questionable, find examples elsewhere or run by James
    [System.Serializable]
    public struct RacerInfo
    {
        public BaseVehicle VehicleSelected;
        public int CharacterSelected;
        public bool IsPlayer;
    }
}
