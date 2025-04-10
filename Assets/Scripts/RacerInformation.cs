using UnityEngine;
using UnityEngine.TextCore.Text;

public class RacerInformation : MonoBehaviour
{
    // questionable, find examples elsewhere
    [System.Serializable]
    public struct RacerInfo
    {
        public BaseVehicle VehicleSelected;
        public int CharacterSelected;
        public bool IsPlayer;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
