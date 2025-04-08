using UnityEngine;

// All the relevant input data for the "actor" in the world
public struct ActorInputData
{
    public float Accelerate;
    public float Brake;
    // Alternatively
    // public float VerticalInput;

    public float TurnInput;

    public bool Jump;
    public bool StuntA;
    public bool StuntB;
    public bool StuntC;
    public bool Drift;
    public bool BoostRam;
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
