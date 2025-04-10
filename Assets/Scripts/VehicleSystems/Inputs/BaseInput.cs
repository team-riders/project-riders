using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// All the relevant input data for the "actor" in the world
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
            { Values.ButtonNamesShort.Accelerate, Accelerate },
            { Values.ButtonNamesShort.Brake, Brake },
            { Values.ButtonNamesShort.TurnInput, TurnInput },
            { Values.ButtonNamesShort.Jump, Jump ? 1 : 0 },
            { Values.InputNameSpecial.JumpHoldDuration, JumpHoldDuration },
            { Values.StuntButtonNamesShort.StuntA, StuntA ? 1 : 0 },
            { Values.StuntButtonNamesShort.StuntB, StuntB ? 1 : 0 },
            { Values.StuntButtonNamesShort.StuntC, StuntC ? 1 : 0 },
            { Values.ButtonNamesShort.Drift, Drift ? 1 : 0 },
            { Values.ButtonNamesShort.BoostRam, BoostRam ? 1 : 0 }
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

public interface IInput
{
    public ActorInputData GrabCurrentFrameInputs();
}

public abstract class BaseInput : MonoBehaviour, IInput
{
    // 3 Major components will be using this
    // StuntSystem, VehicleSystem and Analytics.
    // The AI Will have it's own "AIInput" class that will implement this interface.
    public abstract ActorInputData GrabCurrentFrameInputs();
}
