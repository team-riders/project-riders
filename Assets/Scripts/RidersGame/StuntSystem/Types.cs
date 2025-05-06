using RidersRuntime.Input;
using System.Collections.Generic;

namespace RidersRuntime.StuntSystem
{
    public struct TimedInput
    {
        public float time;
        public string key;
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

        public static float timeWindowMax = 0.5f;
    }
}