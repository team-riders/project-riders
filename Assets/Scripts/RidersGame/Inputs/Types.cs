using System.Collections.Generic;
using System.Linq;

namespace RidersRuntime.Data
{
    [System.Serializable]
    public struct ActorInputData
    {
        public float Accelerate;
        public float Brake;
        // Alternatively
        // public float VerticalInput;

        public float TurnInput;
        /// <summary>
        /// This value only returns 1 WHEN IT HAS BEEN PRESSED, NOT HELD DOWN. THIS IS A UNIQUE VALUE.
        /// </summary>
        public bool Jump;
        /// <summary>
        /// This value only returns the FRAME COUNT OF HOW LONG IT HAS BEEN HELD DOWN FOR. THIS IS A UNIQUE VALUE.
        /// </summary>
        public float JumpHoldDuration;
        public bool StuntA;
        public bool StuntB;
        public bool StuntC;
        public bool Drift;
        public bool BoostRam;

        public List<string> KeysThatAreNonZeroExceptAnalog()
        {
            string[] filter = new string[] { "Accelerate", "Brake", "Horizontal" };
            return ReturnAsDictionary()
            .Where(x => x.Value != 0 && !filter.Contains(x.Key))
            .Select(x => x.Key)
            .ToList();
        }

        public Dictionary<string, float> ReturnAsDictionary()
        {
            return new()
        {
            { ButtonNamesShort.Accelerate, Accelerate },
            { ButtonNamesShort.Brake, Brake },
            { ButtonNamesShort.TurnInput, TurnInput },
            { ButtonNamesShort.Jump, Jump ? 1 : 0 },
            { InputNameSpecial.JumpHoldDuration, JumpHoldDuration },
            { StuntButtonNamesShort.StuntA, StuntA ? 1 : 0 },
            { StuntButtonNamesShort.StuntB, StuntB ? 1 : 0 },
            { StuntButtonNamesShort.StuntC, StuntC ? 1 : 0 },
            { ButtonNamesShort.Drift, Drift ? 1 : 0 },
            { ButtonNamesShort.BoostRam, BoostRam ? 1 : 0 }
        };
        }

        public override string ToString()
        {
            return string.Join(", ", ReturnAsDictionary().Select(x => $"{x.Key}: {x.Value}"));
        }

        public bool AreAnyOfThesePressed(string[] keys)
        {
            Dictionary<string, float> inputData = ReturnAsDictionary();
            foreach (var key in keys)
            {
                if (ReturnAsDictionary().ContainsKey(key) && inputData[key] > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}