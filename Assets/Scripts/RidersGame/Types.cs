using System;

/** 
  * Try to keep UnityEngine references out of here. 
*/

namespace RidersCore.Data
{
    /// <summary>
    /// All the relevant input data for the "actor" in the world
    /// </summary>
    [Serializable]
    public struct ActorInputData
    {
        public float Accelerate;
        public float Brake;
        // Alternatively
        // public float VerticalInput;

        public float TurnInput;

        public bool Jump;
        public float JumpHoldDuration;
        public bool StuntA;
        public bool StuntB;
        public bool StuntC;
        public bool Drift;
        public bool BoostRam;
    }

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