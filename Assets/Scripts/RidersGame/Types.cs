using System;

/** 
  * Try to keep UnityEngine references out of here. 
*/

namespace RidersRuntime.Data
{
    [Serializable]
    public struct InputFrameRecord
    {
        public int Frame;
        public ActorInputData Input;

        public InputFrameRecord(int frame, ActorInputData input)
        {
            Frame = frame;
            Input = input;
        }
    }

    [Serializable]
    public struct RacerInformation
    {
        public int VehicleSelected;
        public int CharacterSelected;
        public bool IsPlayer;
    }
}