using System;
// questionable, find examples elsewhere or run by James
namespace RidersCore.Data
{
    [Serializable]
    public struct RacerInformation
    {
        public int VehicleSelected;
        public int CharacterSelected;
        public bool IsPlayer;
    }
}