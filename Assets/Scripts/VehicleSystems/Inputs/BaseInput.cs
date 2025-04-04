using UnityEngine;
using UnityEngine.InputSystem;

public struct InputData
{
    // figure out what inputs we need
    public InputAction Accelerate;
    public InputAction Brake;
    public InputAction TurnInput;
    public InputAction Jump;
}

//public struct stuff
//{
//    isTryingToJump = true;
//        turnIntentions = ????
//        etc.
//}

public interface IInput
{
    InputData GenerateInput();
}

public abstract class BaseInput : MonoBehaviour, IInput
{
    // override the function to generate input used to steer and control the vehicle
    public abstract InputData GenerateInput();
}
