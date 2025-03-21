using UnityEngine;

public struct InputData
{
    // figure out what inputs we need
    public bool Accelerate;
    public bool Brake;
    public float TurnInput;
    public bool Jump;
}

public interface IInput
{
    InputData GenerateInput();
}

public abstract class BaseInput : MonoBehaviour, IInput
{
    // override the function to generate input used to steer and control the vehicle
    public abstract InputData GenerateInput();
}
