using System.Collections.Generic;
using System.Linq;

namespace RidersRuntime.Input
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
    public enum StuntButtons
    {
        StuntA,
        StuntB,
        StuntC
    }

    public struct StuntButtonNamesFull
    {
        public const string StuntA = "Player/StuntA";
        public const string StuntB = "Player/StuntB";
        public const string StuntC = "Player/StuntC";
    }

    public struct StuntButtonNamesShort
    {
        public const string StuntA = "StuntA";
        public const string StuntB = "StuntB";
        public const string StuntC = "StuntC";
        public readonly static string[] AllStuntButtons = { StuntA, StuntB, StuntC };
    }

    public struct InputNameSpecial
    {
        public const string JumpHoldDuration = "JumpHoldDuration";
    }

    public struct ButtonNamesFull
    {
        public const string Jump = "Player/Jump";
        public const string Accelerate = "Player/Accelerate";
        public const string Brake = "Player/Brake";
        public const string TurnInput = "Player/Horizontal";
        public const string Drift = "Player/Drift";
        public const string BoostRam = "Player/BoostRam";
        public const string PauseButton = "Player/PauseButton";
    }

    public struct ButtonNamesShort
    {
        public const string Jump = "Jump";
        public const string Accelerate = "Accelerate";
        public const string Brake = "Brake";
        public const string TurnInput = "Horizontal";
        public const string Drift = "Drift";
        public const string BoostRam = "Boost/Ram";
        public const string PauseButton = "PauseButton";
    };

    public struct InputMap
    {
        public const string Player = "Player";
        public const string UI = "UI";
        public const string Menu = "Menu";
        public const string Debug = "Debug";
    }
}