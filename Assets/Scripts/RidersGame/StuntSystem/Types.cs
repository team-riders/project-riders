using RidersRuntime.Input;
using System.Collections.Generic;

namespace RidersRuntime.StuntSystem
{
    public struct FrameKeyData
    {
        public int frameCount;
        public List<string> inputs;
    }

    public enum StuntType
    {
        None,
        Jump,
        Spin,
        Flip,
        Grind
    }

    public static class Values
    {
        public static readonly Dictionary<string, StuntType> stuntBindings = new() {
            { StuntButtonNamesShort.StuntA, StuntType.Flip },
            { StuntButtonNamesShort.StuntB, StuntType.Grind },
            { StuntButtonNamesShort.StuntC, StuntType.Spin }
        };
    }
}