using RidersRuntime.Input;
using System;

namespace RidersRuntime.Analytics
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
}