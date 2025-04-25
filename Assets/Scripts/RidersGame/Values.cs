using System.Collections.Generic;
using RidersCore.Data;

namespace RidersCore.StuntSystem
{
    public static class Values
    {
        public static readonly Dictionary<string, StuntType> stuntBindings = new() {
            { StuntButtonNamesShort.StuntA, StuntType.Flip },
            { StuntButtonNamesShort.StuntB, StuntType.Grind },
            { StuntButtonNamesShort.StuntC, StuntType.Spin }
        };
    }
}