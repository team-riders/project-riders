using UnityEngine;
using UnityEngine.InputSystem;

public struct InputData
{
    public InputAction Accelerate;
    public InputAction Brake;
    public InputAction TurnInput;
    public InputAction Jump;
    
    public InputAction TrickButtonA;
    public InputAction TrickButtonB;
    public InputAction TrickButtonC;

    public InputAction PauseButton;

    public InputAction DriftButton;
    public InputAction BoostRamButton;
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
